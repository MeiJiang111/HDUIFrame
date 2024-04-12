using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInitialize : MonoSingleton<GameInitialize>
{
    [Serializable]
    public struct LoadPrefabConfig
    {
        public string name;
        public Vector3 pos;
    }

    public bool ShowFrame;           //��ʾ֡��        
    public int TargetFrame;          //�޶�֡��      
    public bool ShowDebugGrid;

    [Header("��������")] 
    public bool update;
    public List<LoadPrefabConfig> prefabList;
    public event Action GameInitEvent;

    bool _gameInit;

    protected override void Awake()
    {
        base.Awake();

        _gameInit = false;
        Application.targetFrameRate = TargetFrame;
        Application.runInBackground = true;

        if (ShowFrame)
        {
            gameObject.AddComponent<FrameRate>();
        }
    }

    void Start()
    {
        GameUpdate.Instance.StartGameUpdate(update);
    }

   
    void Update()
    {
        
    }
}
