using Async.UIFramework;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// ������ʼ���ű�
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

        //ע����Դ�����ͷ��¼�
        UIFrame.OnAssetRequest += OnAssetRequest;
        UIFrame.OnAssetRelease += OnAssetRelease;
       
        //ע��UI��ס�¼�
        //����ʱ�䳬��0.5s�󴥷�UI��ס�¼�
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
    /// ��Դ�ͷ��¼�
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
