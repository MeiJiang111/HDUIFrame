using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AsyncPrefabInfo
{
    public string Name;
    public Vector3 Pos;
}

/// 场景初始化脚本
public class LevelInitializer : MonoBehaviour
{
    public List<AsyncPrefabInfo> asyncPrefabs;
    
    private void Awake()
    {
        LevelManager.Instance.RegisterLoadPrefabs(asyncPrefabs, CreatPrefab, CreatPrefabFaild);
    }

    private void CreatPrefab(string name, GameObject obj, object parmas_)
    {
        foreach (var item in asyncPrefabs)
        {
            if (item.Name.Equals(name))
            {
                obj.transform.localPosition = item.Pos;
            }
        }
    }

    private void CreatPrefabFaild(string name_)
    {
        LogUtil.LogError("load scene creat prefab faild ...");
    }
}
