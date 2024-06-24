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
        Debug.Log("ddd -- LevelInitializer - Awake");
    }

    private void Start()
    {
        Debug.Log("ddd -- LevelInitializer - Start");

        //注册资源请求释放事件
        UIFrame.OnAssetRequest += OnAssetRequest;
        UIFrame.OnAssetRelease += OnAssetRelease;
       
        //注册UI卡住事件
        //加载时间超过0.5s后触发UI卡住事件
        UIFrame.StuckTime = 0.5f;
        UIFrame.OnStuckStart += OnStuckStart;
        UIFrame.OnStuckEnd += OnStuckEnd;
        UIFrame.Show<LoginPanel>();
    }

    private async UniTask<GameObject> OnAssetRequest(Type type)
    {
        Debug.Log("ddd -- LevelInitializer - OnAssetRequest");
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
