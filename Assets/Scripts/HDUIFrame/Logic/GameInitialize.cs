using System;
using System.Collections;
using UnityEngine;

public class GameInitialize : MonoSingleton<GameInitialize>
{
    public bool ShowFrame;           //是否显示帧数        
    public int TargetFrame;          //限定帧数      
    
    [Header("开启更新")] 
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
