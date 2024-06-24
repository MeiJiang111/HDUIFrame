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
   
    protected override void Awake()
    {
        base.Awake();

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
        LevelManager.Instance.StartLevel(Global.LOGIN_LEVEL_NAME);
    }
}
