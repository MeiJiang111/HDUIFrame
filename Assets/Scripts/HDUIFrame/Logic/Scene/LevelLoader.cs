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
    string _lastLevel = string.Empty;
    bool _actived;
    
    /// <summary>
    /// 场景在加载中
    /// </summary>
    public bool InLoading { get; private set; }
    public string CurLevel { get; private set; }
    public string LoadingLevel => _newLevel;
    public bool AutoActive { get; private set; }
   
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
            LogUtil.LogWarningFormat("{0} new scene is loading ,can not load new scene!", _newLevel);
            return;
        }

        LevelStartLoadEvent?.Invoke();    //todo
        _lastLevel = _newLevel;
        _newLevel = name_;
        _preScene = _loadScene;
        AutoActive = autoActive_;
        _actived = false;

        Addressables.LoadSceneAsync(_newLevel, LoadSceneMode.Single, AutoActive).Completed += OnLevelLoaded;
    }

    /// <summary>
    /// 场景加载
    /// </summary>
    /// <param name="handle_"></param>
    void OnLevelLoaded(AsyncOperationHandle<SceneInstance> handle_)
    {
        InLoading = false;

        if (handle_.Status != AsyncOperationStatus.Succeeded)
        {
            LogUtil.LogErrorFormat("LoadNewScene {0} failed", _newLevel);
            _newLevel = string.Empty;
            return;
        }

        _loadScene = handle_.Result;
        if (AutoActive)
        {
            _actived = true;
            CurLevel = _newLevel;
            _newLevel = string.Empty;

            Debug.Log($"ddd -- LevelLoader OnLevelLoaded");
            if (!string.IsNullOrEmpty(_lastLevel))
            {
                Debug.Log($"ddd -- LevelLoader OnLevelLoaded {_preScene.Scene.name}");
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
            LogUtil.LogWarning("can not need active Level!");
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
