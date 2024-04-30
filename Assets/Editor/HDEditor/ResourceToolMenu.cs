using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class ResourceToolMenu
{
    [MenuItem("资源管理/生成excel配置", false, 1)]
    public static void BuildExcelConfig()
    {
        Debug.LogError("TODO");
    }

    [MenuItem("资源管理/构建所有资源配置(AB)", false, 101)]
    public static void BuildResourcePathObj()
    {
        AddressableGroupBuild.BuildAddressableGroupPath();
    }
}
