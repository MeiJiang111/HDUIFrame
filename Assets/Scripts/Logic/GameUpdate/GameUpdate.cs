using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
#if UNITY_EDITOR
        if (!update)
        {
            CurrState = UpdateState.Finish;
            //编辑器下可以跳过更新
            UpdateFinished();
            return;
        }
#endif
        ResourceManager.Instance.AddressableErrorEvent += OnAddressableErrored;
        updateCoroution = StartCoroutine(StartGameUpdateImple());
    }

    void UpdateFinished()
    {
        StartCoroutine(GameInitialize.Instance.EnterGame());
    }

}
