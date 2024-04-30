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



public class LevelManager : MonoSingleton<LevelManager>
{
    /// <summary>
    /// 开始加载新的场景回调
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

        levelLoader = LevelLoader.Instance;
        levelAsyncPrefabs = new List<AsyncPrefabs>();

        //levelLoader.LevelActivedEvent += OnLevelActived;
        //levelLoader.LevelStartLoadEvent += OnLevelStartLoad;
        //levelLoader.LevelLoadedEvent += OnLevelLoaded;
    }

    public bool StartLevel(string name_, bool autoActive = true)
    {
        Debug.Log("ddd -- LevelManager -- StartLevel() == " + levelLoader.InLoading);
        Debug.Log($"ddd -- LevelManager -- StartLevel() == name_ == {name_}");

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
        StartLoadingNewLevelEvent?.Invoke(_newLevel);
        levelLoader.LoadLevelAsync(_newLevel, _autoActive);
    }
}
