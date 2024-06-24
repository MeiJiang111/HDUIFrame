using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ����������
/// </summary>
public class LevelManager : MonoSingleton<LevelManager>
{
    /// <summary>
    /// ��ʼ�����µĳ���ǰ�Ļص�
    /// </summary>
    public Action<string> StartLoadingNewLevelEvent;
    
    /// <summary>
    /// ������ʼ����ʱ��Ļص�
    /// </summary>
    public Action LevelLoadedEvent;

    /// <summary>
    /// ���������еĻص�
    /// </summary>
    public Action LevelPreStartEvent;

    /// <summary>
    /// ������ʼ����ʱ��Ļص�
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
   
    bool _autoActive;

    LevelLoader levelLoader;
  
    int _levelStartWaitCount = 0;
    
    protected override void Awake()
    {
        base.Awake();

        levelLoader = LevelLoader.Instance;
        levelLoader.LevelStartLoadEvent += OnLevelStartLoad;
        levelLoader.LevelLoadedEvent += OnLevelLoaded;
        levelLoader.LevelActivedEvent += OnLevelActived; 
    }

    public bool StartLevel(string name_, bool autoActive = true)
    {
        Debug.Log($"ddd -- LevelManager StartLevel  --  (new scene name) == {name_}");
        if (levelLoader.InLoading)
        {
            LogUtil.LogWarningFormat("Call attempted to LoadLevel {0} while a level is already in the process of loading; ignoring the load request...", levelLoader.LoadingLevel);
            return false;
        }

        _isStart = false;
        _autoActive = autoActive;
        _newLevel = name_;
        _levelStartWaitCount = 0;
        asyncLoadedNum = 0;

        StopAllCoroutines();
        StartCoroutine(StartLevelImple());
        return true;
    }

    IEnumerator StartLevelImple()
    {
        yield return null;

        StartLoadingNewLevelEvent?.Invoke(_newLevel);   //todo 
        levelLoader.LoadLevelAsync(_newLevel, _autoActive);
    }

    //-------------------------------- ������ʼ����ǰ --------------------------------
    private void OnLevelStartLoad()
    {
        Debug.Log($"ddd -- LevelManager ready to start loading  {_newLevel} ������δ��ʼ���� ...");
    }

    //-------------------------------- ���������лص� --------------------------------
    private void OnLevelLoaded()
    {
        CurLevel = _newLevel;
   
        Debug.Log($"ddd -- LevelManager OnLevelLoaded -- levelLoader.AutoActive == {levelLoader.AutoActive} _autoActive == {_autoActive}");
        if (!levelLoader.AutoActive && _autoActive)
        {
            Debug.Log("ddd -- ����ô���� ...");
            StartCoroutine(levelLoader.ActiveLevel());
        }
    }

    //-------------------------------- �������ؽ����ص� --------------------------------
    private void OnLevelActived()
    {
        _autoActive = false;
        StartCoroutine(LevelStart());
    }

    IEnumerator LevelStart()
    {
        yield return null;
        _isStart = false;
        LevelPreStartEvent?.Invoke();

        //while (LevelStartPaused)
        //{
        //    yield return null;
        //}

        yield return new WaitForEndOfFrame();
        _isStart = true;
        LevelStartEvent?.Invoke();
    }
}
