using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartTest : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    public GameObject item;
    public int totalCount = -1;

    Stack<Transform> pool = new Stack<Transform>();

    void Start()
    {
        var ls = GetComponent<LoopScrollRect>();
        ls.prefabSource = this;
        ls.dataSource = this;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }


    void Update()
    {
        
    }

    public GameObject GetObject(int index)
    {
        if(pool.Count == 0)
        {
            return Instantiate(item);
        }

        Transform candidate = pool.Pop();
        candidate.gameObject.SetActive(true);
        return candidate.gameObject;
    }

    public void ReturnObject(Transform trans)
    {
        Debug.Log("ddd -- ReturnObject == " + transform);
        trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {
        //Debug.Log("ddd -- ProvideData == idx    " + idx);
        transform.SendMessage("ScrollCellIndex", idx);
    }
}
