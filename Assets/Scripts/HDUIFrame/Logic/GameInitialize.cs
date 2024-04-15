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

    public bool ShowFrame;           //显示帧数        
    public int TargetFrame;          //限定帧数      
    public bool ShowDebugGrid;

    [Header("开启更新")] 
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
