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

    bool _isStart;
    bool _autoActive;
    LevelLoader levelLoader;
  
    protected override void Awake()
    {
        base.Awake();

        levelLoader = LevelLoader.Instance;
        levelLoader.LevelStartLoadEvent += OnLevelStartLoad;
        levelLoader.LevelLoadedEvent += OnLevelLoaded;
        levelLoader.LevelActivedEvent += OnLevelActived; 
    }

    public void StartLevel(string name_, bool autoActive = true)
    {
        Debug.Log($"ddd -- LevelManager StartLevel  --  (new scene name) == {name_}");
        if (levelLoader.InLoading)
        {
            LogUtil.LogWarningFormat("Call attempted to LoadLevel {0} while a level is already in the process of loading; ignoring the load request...", levelLoader.LoadingLevel);
            return;
        }

        _isStart = false;
        _autoActive = autoActive;
        _newLevel = name_;
    
        StopAllCoroutines();
        StartCoroutine(StartLevelImple());
    }

    IEnumerator StartLevelImple()
    {
        yield return null;
        levelLoader.LoadLevelAsync(_newLevel, _autoActive);
    }

    //-------------------------------- ������ʼ����ǰ --------------------------------
    private void OnLevelStartLoad()
    {
        //Debug.Log($"ddd -- LevelManager ready to start loading  {_newLevel} ������δ��ʼ���� ...");
    }

    //-------------------------------- ���������лص� --------------------------------
    private void OnLevelLoaded()
    {
        CurLevel = _newLevel;
  
        if (!levelLoader.AutoActive && _autoActive)
        {
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
        _isStart = true;
        LevelStartEvent?.Invoke();
    }
}
