using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelLoader : MonoSingleton<LevelLoader>
{
    public Action LevelStartLoadEvent;
    public Action LevelLoadedEvent;
    public Action LevelActivedEvent;

    SceneInstance _loadScene;
    SceneInstance _preScene;

    string _newLevel = string.Empty;
    bool _actived;
    string _lastLevel = string.Empty;

    /// <summary>
    /// �����ڼ�����
    /// </summary>
    public bool InLoading { get; private set; }
    public string CurLevel { get; private set; }
    public string LoadingLevel => _newLevel;
    public bool AutoActive { get; private set; }
    public bool LevelActived => _actived;
   

    protected override void Awake()
    {
        base.Awake();

        InLoading = false;
        _lastLevel = string.Empty;
    }

    public void LoadLevelAsync(string name_, bool autoActive_ = true)
    {
        if (InLoading)
        {
            LogUtil.LogWarningFormat("{0} level is loading ,can not load new level!", _newLevel);
            return;
        }

        LevelStartLoadEvent?.Invoke();
        _lastLevel = _newLevel;
        _newLevel = name_;
        _preScene = _loadScene;
        AutoActive = autoActive_;
        _actived = false;

        Addressables.LoadSceneAsync(_newLevel, LoadSceneMode.Single, AutoActive).Completed += OnLevelLoaded;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="handle_"></param>
    void OnLevelLoaded(AsyncOperationHandle<SceneInstance> handle_)
    {
        InLoading = false;

        if (handle_.Status != AsyncOperationStatus.Succeeded)
        {
            LogUtil.LogErrorFormat("LoadLevel {0} failed", _newLevel);
            _newLevel = string.Empty;
            return;
        }

        _loadScene = handle_.Result;
        if (AutoActive)
        {
            _actived = true;
            CurLevel = _newLevel;
            _newLevel = string.Empty;
            if (!string.IsNullOrEmpty(_lastLevel))
            {
                Addressables.UnloadSceneAsync(_preScene);
                _lastLevel = string.Empty;
            }
        }

        LevelLoadedEvent?.Invoke();

        if (AutoActive)
        {
            StartCoroutine(SendActiveLevelEvent());
        }
    }

    IEnumerator SendActiveLevelEvent()
    {
        yield return null;
        LevelActivedEvent?.Invoke();
    }

    public IEnumerator ActiveLevel()
    {
        yield return null;
        if (_actived)
        {
            LogUtil.LogWarning("can not need activeLevel!");
        }
        else
        {
            var handle = _loadScene.ActivateAsync();
            yield return handle;
            _actived = true;
            CurLevel = _newLevel;
            _newLevel = string.Empty;
            LevelActivedEvent?.Invoke();
        }
    }
}
