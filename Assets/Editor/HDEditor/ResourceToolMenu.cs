using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class ResourceToolMenu
{
    [MenuItem("��Դ����/����excel����", false, 1)]
    public static void BuildExcelConfig()
    {
        Debug.LogError("TODO");
    }

    [MenuItem("��Դ����/����������Դ����(AB)", false, 101)]
    public static void BuildResourcePathObj()
    {
        AddressableGroupBuild.BuildAddressableGroupPath();
    }
}
