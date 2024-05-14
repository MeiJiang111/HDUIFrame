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
    
    [Header("开启更新")] 
    public bool update;
    public List<LoadPrefabConfig> firstLaodPrefabs;
   
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
        var resourMgr = ResourceManager.Instance;
        int count = 0;
        count = firstLaodPrefabs.Count;

        foreach (var item in firstLaodPrefabs)
        {
            resourMgr.CreatInstanceAsync(item.name, (obj, parma) =>
            {
                obj.name = item.name;
                obj.transform.localPosition = item.pos;
                count--;
            });
        }

        while (count > 0)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        //CameraController.Instance.RegisterListenner();
        LogUtil.Log("Game Initialize loading finish！！！");
        LevelManager.Instance.StartLevel(Global.LOGIN_LEVEL_NAME);
    }
}
