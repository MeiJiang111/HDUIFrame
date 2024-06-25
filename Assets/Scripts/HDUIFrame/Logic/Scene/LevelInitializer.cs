using Async.UIFramework;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// 场景初始化脚本
public class LevelInitializer : MonoBehaviour
{
    [SerializeField] 
    private GameObject stuckPanel;

    private void Awake()
    {

    }

    private void Start()
    {
        //注册资源请求释放事件
        UIFrame.OnAssetRequest += OnAssetRequest;
        UIFrame.OnAssetRelease += OnAssetRelease;
       
        //注册UI卡住事件
        //加载时间超过0.5s后触发UI卡住事件
        UIFrame.StuckTime = 0.5f;
        UIFrame.OnStuckStart += OnStuckStart;
        UIFrame.OnStuckEnd += OnStuckEnd;
      
        var data = new LoginData();
        data.user = "dyc";
        data.password = "123";
        UIFrame.Show<LoginPanel>(data);
    }

    private async UniTask<GameObject> OnAssetRequest(Type type)
    {
        var layer = UIFrame.GetLayer(type);
        var handle = Addressables.LoadAssetAsync<GameObject>(type.Name).ToUniTask();
        return await handle;
    }

    /// <summary>
    /// 资源释放事件
    /// </summary>
    /// <param name="type"></param>
    private void OnAssetRelease(Type type)
    {
        //TODO
    }

    private void OnStuckStart()
    {
        //stuckPanel.SetActive(true);
    }

    private void OnStuckEnd()
    {
        //stuckPanel.SetActive(false);
    }
}
