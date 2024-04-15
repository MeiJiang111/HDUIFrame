using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public Action<PanelType> PanelStartCloseEvent;
    public Action<PanelType> PanelStartOpenEvent;
    public Action<PanelType> PanelOpenedFinishEvent;
    public Action<PanelType> PanelClosedFinishEvent;

    public Transform fullScreenRoot;
    public Transform halfScreenRoot;
    public Transform dialogRoot;
    public Transform popInfoRoot;
    public Transform GlobalRoot;

    public Camera UICamera;
    public Material desaturateMaterial;

    Dictionary<PanelType, PanelPrefabConfig> _panelPrefabConfigDict = new Dictionary<PanelType, PanelPrefabConfig>();
    PanelPrefabConfig defaultPanelPrefabConfig = new PanelPrefabConfig() 
    { 
        name = string.Empty, 
        isResident = false 
    };

    public List<PanelType> preLoadingPanels;

    Dictionary<PanelType, OpenClosePanel> cachePanels = new Dictionary<PanelType, OpenClosePanel>();
    //List<OpenClosePanel> curOpendPanel = new List<OpenClosePanel>();
    //List<OpenClosePanel> tempList = new List<OpenClosePanel>();
    //int _waiteCount;
    //public bool HasWaite => _waiteCount > 0;

    //public float CanvasOffset { get; private set; }

    SceneType _curSceneType;
    public SceneType CurUISceneType
    {
        get 
        { 
            return _curSceneType; 
        }

        set
        {
            //_curSceneType = value;
            //var types = cachePanels.Keys.ToArray();
            //for (int i = types.Length - 1; i >= 0; i--)
            //{
            //    var panleType = types[i];
            //    var _config = GetPanelConfig(panleType);
            //    if (!_config.isResident ||
            //        (_config.scene != _curSceneType && _config.scene != SceneType.All))   //非持久化 直接卸载掉
            //    {
            //        var openclose = cachePanels[panleType];
            //        cachePanels.Remove(panleType);

            //        ResourceManager.Instance.DestroyInstance(openclose.gameObject);
            //        LogUtil.LogFormat("destroy panel {0}", panleType);
            //    }
            //}
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
