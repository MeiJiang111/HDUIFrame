using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



// UI���Ļ����ű�
public class OpenClosePanel : MonoBehaviour
{
    public Action<object> PanelStartOpenEvent;
    public Action PanelOpenFinishEvent;
    public Action PanelStartCloseEvent;
    public Action PanelCloseFinishEvent;

    public PanelType type;
    public PanelGroup group;
    public SceneType scene = SceneType.Main;
    public bool isResident = false;

    bool _openFinish;
    bool _closeFinish;
    public bool OpenFinish => _openFinish;
    public bool CloseFinish => _closeFinish;
    public bool IsOpen { get; private set; }

    [Header("Ĭ���Ƿ���")]
    [SerializeField]
    private bool initOpenAwake = false;
    public bool isInitialOpened = false;

    private void Awake()
    {
        if (type != PanelType.None)
        {
            //UIManager.Instance.RegisterPanel(this);
        }

        if (initOpenAwake)
        {
            Init();
        }
            
    }

    void Start()
    {
        if (IsOpen)
        {
            return;
        }

        if (!initOpenAwake)
        {
            Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        if (isInitialOpened)
        {
            OpenPanel();
        }
        else
        {
            ClosePanelImple();
        }  
    }

    public void OpenPanel(object param_ = null)
    {
        if (IsOpen) 
        {
            return;
        }
        
        IsOpen = true;
        _openFinish = false;
        PanelStartOpenEvent?.Invoke(param_);

        if (type != PanelType.None)
        {
            //UIManager.Instance.PanelStartOpen(this);
        }

        //�������嶯Ч
        //todo
        IsOpen = true;
        OpenPanelImple();
        OnPanelOpenFinish();
    }

    private void OpenPanelImple()
    {
        gameObject.SetActive(true);
    }

    private void ClosePanelImple()
    {
        gameObject.SetActive(false);
    }

    private void OnPanelOpenFinish()
    {
        _openFinish = true;
        PanelOpenFinishEvent?.Invoke();
        if (type != PanelType.None)
        {
            //UIManager.Instance.PanelOpenFinished(this);
        }
    }

    private void OnPanelColseFinish()
    {
        if (IsOpen)
        {
            return;
        }

        if (_closeFinish)
        {
            return;
        }
        _closeFinish = true;

        ClosePanelImple();

        PanelCloseFinishEvent?.Invoke();

        if (type != PanelType.None)
        {
            var uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                //uiManager.PanelCloseFinised(this);
            }
                
        }
    }

    public void ClosePanel()
    {
        if (!IsOpen) return;

        IsOpen = false;
        _closeFinish = false;
        PanelStartCloseEvent?.Invoke();
        if (type != PanelType.None)
        {
            var uiManager = UIManager.Instance;
            if (uiManager != null)
            {
                //uiManager.PanelStartClose(this);
            }
        }

        OnPanelColseFinish();
    }
}
