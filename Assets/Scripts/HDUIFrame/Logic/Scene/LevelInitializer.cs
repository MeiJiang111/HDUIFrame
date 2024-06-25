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

    }

    private void Start()
    {
        //ע����Դ�����ͷ��¼�
        UIFrame.OnAssetRequest += OnAssetRequest;
        UIFrame.OnAssetRelease += OnAssetRelease;
       
        //ע��UI��ס�¼�
        //����ʱ�䳬��0.5s�󴥷�UI��ס�¼�
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
