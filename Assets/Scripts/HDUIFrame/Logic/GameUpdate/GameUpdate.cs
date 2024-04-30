using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class GameUpdate : MonoSingleton<GameUpdate>
{
    public enum UpdateState
    {
        None,
        Init,
        VerifyVersion,
        VerifyVersionSuccess,
        Download,
        Finish,
        Failed,
    }

    public Action<UpdateState> UpdateStateChangedEvent;
    public Action<float> DownLoadProcessChangeEvent;

    string _lastName;
    string _lastErr;

    /// <summary>
    /// ��Դ������־
    /// </summary>
    List<string> updateCatalogs; 
    Coroutine updateCoroution;

    UpdateState _state;
    public UpdateState CurrState
    {
        get
        { 
            return _state; 
        }

        set
        {
            _state = value;
            UpdateStateChangedEvent?.Invoke(_state);
        }
    }

    protected override void Awake()
    {
        _state = UpdateState.None;
        updateCatalogs = new List<string>();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void StartGameUpdate(bool update = true)
    {
        ResourceManager.Instance.AddressableErrorEvent += OnAddressableErrored;
        updateCoroution = StartCoroutine(StartGameUpdateImple());
    }

    private void OnAddressableErrored(AsyncOperationHandle arg1, Exception arg2)
    {
        _lastName = arg1.DebugName;
        _lastErr = arg2.ToString();
    }

    /// <summary>
    /// ��Ϸ������ɣ�������Ϸ
    /// </summary>
    void UpdateFinished()
    {
        StartCoroutine(GameInitialize.Instance.EnterGame());
    }

    IEnumerator StartGameUpdateImple()
    {
        CurrState = UpdateState.Init;
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;
       
        if (!string.IsNullOrEmpty(_lastName))
        {
            CurrState = UpdateState.Failed;
            LogUtil.LogErrorFormat("[GameUpdate]: int faild by {0} ", _lastErr);
            StopCoroutine(updateCoroution);
        }
        yield return new WaitForEndOfFrame();

        CurrState = UpdateState.VerifyVersion;
        var handler = Addressables.CheckForCatalogUpdates(false);
        yield return handler;

        if (handler.Status != AsyncOperationStatus.Succeeded || (!string.IsNullOrEmpty(_lastName)))
        {
            CurrState = UpdateState.Failed;
            LogUtil.LogErrorFormat("[GameUpdate]: CheckForCatalogUpdates faild by {0} ", _lastErr);
            StopCoroutine(updateCoroution);
        }
        yield return new WaitForEndOfFrame();

        updateCatalogs = handler.Result;
        Debug.Log("ddd -- updateCatalogs.count == " + updateCatalogs.Count);
        Addressables.Release(handler);
        if (updateCatalogs.Count > 0)
        {
            CurrState = UpdateState.Download;
            updateCoroution = StartCoroutine(StartDownload());
        }
        else
        {
            CurrState = UpdateState.Finish;
            UpdateFinished();
        }
    }

    IEnumerator StartDownload()
    {
        Debug.Log("ddd -- StartDownload");
        var updateHandler = Addressables.UpdateCatalogs(updateCatalogs, false);
        yield return updateHandler;

        if (updateHandler.Status != AsyncOperationStatus.Succeeded || (!string.IsNullOrEmpty(_lastName)))
        {
            CurrState = UpdateState.Failed;
            LogUtil.LogErrorFormat("[GameUpdate]: UpdateCatalogs faild by {0}", _lastErr);
            StopCoroutine(updateCoroution);
        }
        yield return new WaitForEndOfFrame();

        CurrState = UpdateState.VerifyVersionSuccess;
        var locators = updateHandler.Result;
        foreach (var locator in locators)
        {
            var downloadHandle = Addressables.DownloadDependenciesAsync(locator.Keys, Addressables.MergeMode.Union);
            while (!downloadHandle.IsDone)
            {
                DownLoadProcessChangeEvent?.Invoke(downloadHandle.PercentComplete);
                yield return new WaitForEndOfFrame();
            }
            Addressables.Release(downloadHandle);
        }
        CurrState = UpdateState.Finish;
        Addressables.Release(updateHandler);
        DownLoadProcessChangeEvent?.Invoke(1);
        yield return null;

        //��Դ����������ɣ�������Ϸ
        UpdateFinished();
    }
}
