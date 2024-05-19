using System;
using System.Collections;
using UnityEngine;

public class GameInitialize : MonoSingleton<GameInitialize>
{
    public bool ShowFrame;           //�Ƿ���ʾ֡��        
    public int TargetFrame;          //�޶�֡��      
    
    [Header("��������")] 
    public bool update;
   
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

    public IEnumerator EnterGame()
    {
        yield return null;
        LogUtil.Log("GameInitialize EnterGame finish !!!");
        //LevelManager.Instance.StartLevel(Global.LOGIN_LEVEL_NAME);
    }
}
