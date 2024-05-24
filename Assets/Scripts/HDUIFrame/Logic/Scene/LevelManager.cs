using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AsyncPrefabs
{
    public string name;
    public Action<string, GameObject, object> CreatSuccess;
    public Action<string> CreatFaild;
}


/// <summary>
/// 场景管理器
/// </summary>
public class LevelManager : MonoSingleton<LevelManager>
{
    /// <summary>
    /// 开始加载新的场景前的回调
    /// </summary>
    public Action<string> StartLoadingNewLevelEvent;
    
    /// <summary>
    /// 场景开始加载时候的回调
    /// </summary>
    public Action LevelLoadedEvent;

    /// <summary>
    /// 场景加载中的回调
    /// </summary>
    public Action LevelPreStartEvent;

    /// <summary>
    /// 场景开始加载时候的回调
    /// </summary>
    public Action LevelStartEvent;

    string _newLevel;

    public string CurLevel
    {
        get;
        private set;
    }

    public bool IsMainLevel => (CurLevel == Global.MAIN_LEVEL_NAME);

    bool _isStart;
    public bool LevelIsStart => _isStart;

    int asyncLoadedNum;
    List<AsyncPrefabs> levelAsyncPrefabs;
    public float AsyncLoadingPct => asyncLoadedNum * 1f / levelAsyncPrefabs.Count;

    bool _autoActive;
    LevelLoader levelLoader;
  
    int _levelStartWaitCount = 0;
    public bool LevelStartPaused
    {
        get
        {
            if (_levelStartWaitCount > 0)
            {
                return true;
            }

            return asyncLoadedNum < levelAsyncPrefabs.Count;
        }
    }

    protected override void Awake()
    {
        base.Awake();
       
        levelAsyncPrefabs = new List<AsyncPrefabs>();
        levelLoader = LevelLoader.Instance;
        levelLoader.LevelStartLoadEvent += OnLevelStartLoad;
        levelLoader.LevelLoadedEvent += OnLevelLoaded;
        levelLoader.LevelActivedEvent += OnLevelActived; 
    }

    public void RegisterLoadPrefabs(List<AsyncPrefabInfo> prefabs, Action<string, GameObject, object> success, Action<string> faild = null)
    {
        foreach (var item in prefabs)
        {
            levelAsyncPrefabs.Add(new AsyncPrefabs() { name = item.Name, CreatSuccess = success, CreatFaild = faild});
        }
    }

    public bool StartLevel(string name_, bool autoActive = true)
    {
        Debug.Log($"ddd -- LevelManager StartLevel --  (new scene name) == {name_}");
        if (levelLoader.InLoading)
        {
            LogUtil.LogWarningFormat("Call attempted to LoadLevel {0} while a level is already in the process of loading; ignoring the load request...", levelLoader.LoadingLevel);
            return false;
        }

        _isStart = false;
        _autoActive = autoActive;
        _newLevel = name_;
        _levelStartWaitCount = 0;
        levelAsyncPrefabs.Clear();
        asyncLoadedNum = 0;

        StopAllCoroutines();
        StartCoroutine(StartLevelImple());
        return true;
    }

    IEnumerator StartLevelImple()
    {
        yield return null;
        StartLoadingNewLevelEvent?.Invoke(_newLevel);   //todo 
        yield return null;
        levelLoader.LoadLevelAsync(_newLevel, _autoActive);
    }

    //-------------------------------- 场景开始加载前 --------------------------------
    private void OnLevelStartLoad()
    {
        Debug.Log($"ddd -- LevelManager ready to start loading scene {_newLevel} 场景还未开始加载 ...");
    }

    //-------------------------------- 场景加载中回调 --------------------------------
    private void OnLevelLoaded()
    {
        CurLevel = _newLevel;
   
        Debug.Log($"ddd -- LevelManager OnLevelLoaded -- levelLoader.AutoActive == {levelLoader.AutoActive} _autoActive == {_autoActive}");
        if (!levelLoader.AutoActive && _autoActive)
        {
            Debug.Log("ddd -- 根本么进来 ...");
            StartCoroutine(levelLoader.ActiveLevel());
        }
    }

    //-------------------------------- 场景加载结束回调 --------------------------------
    private void OnLevelActived()
    {
        Debug.Log($"ddd -- LevelManager OnLevelActived == count == {levelAsyncPrefabs.Count}");
        foreach (var item in levelAsyncPrefabs)
        {
            ResourceManager.Instance.CreatInstanceAsync(item.name, CreatPrefabSuccess, CreatPrefabFaild);
        }
        _autoActive = false;
        StartCoroutine(LevelStart());
    }

    void CreatPrefabSuccess(GameObject obj, object parmas = null)
    {
        asyncLoadedNum++;
        string trueName = obj.name.Replace(Global.Clone_Str, "");
        var _info = levelAsyncPrefabs.Find(new Predicate<AsyncPrefabs>((AsyncPrefabs) =>
        {
            if(AsyncPrefabs.name == trueName)
            {
                return true;
            }
            return true;
        }));

        if (string.IsNullOrEmpty(_info.name))
        {
            LogUtil.LogWarningFormat("Creat prefab {0} success but not exists!!!", trueName);
            return;
        }
        _info.CreatSuccess?.Invoke(trueName, obj, parmas);
    }

    void CreatPrefabFaild(string name)
    {
        asyncLoadedNum++;
        var _info = levelAsyncPrefabs.Find(new Predicate<AsyncPrefabs>((AsyncPrefabs) =>
        {
            if (AsyncPrefabs.name == name)
            {
                return true;
            }
            return false;
        }));

        if (string.IsNullOrEmpty(_info.name))
        {
            LogUtil.LogWarningFormat("Creat prefab {0} Faild but not exists!!!", name);
            return;
        }
        _info.CreatFaild?.Invoke(name);
    }

    IEnumerator LevelStart()
    {
        yield return null;
        _isStart = false;
        LevelPreStartEvent?.Invoke();

        while (LevelStartPaused)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        _isStart = true;
        LevelStartEvent?.Invoke();
    }
}
