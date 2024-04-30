using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;



public class AddressableGroupBuild 
{
    public struct AssetInfo
    {
        public string FilePath;
        public string GroupName;
        public string FileName;
        public string LabelName;

        public AssetInfo(string path_)
        {
            FilePath = path_;
            FileName = Path.GetFileNameWithoutExtension(path_);
            var tempPath = path_.Replace(EditorPath.BUILD_RES_ROOT, "");
      
            var index = tempPath.IndexOf('/');
            if (index < 0)
            {
                Debug.LogError(path_);
            }

            GroupName = tempPath.Substring(0, tempPath.IndexOf('/'));
            LabelName = Path.GetDirectoryName(path_).Replace("\\", "/").Replace(EditorPath.BUILD_RES_ROOT, "").Replace("/", "_");
        }
    }

    static AddressableAssetSettings setting 
    { 
        get { return AddressableAssetSettingsDefaultObject.Settings; } 
    }

    public static void BuildAddressableGroupPath()
    {
        List<AssetInfo> assetsList = new List<AssetInfo>();

        var files = Directory.GetFiles(EditorPath.BUILD_RES_ROOT, "*", SearchOption.AllDirectories);
        int total = files.Length;
        float index = 1f;

        foreach (var file in files)
        {
            if (file.Contains(".meta") || file.Contains(".exr") || file.Contains("tpsheet") || file.Contains(".DS_Store") || file.Contains("/Template") || file.Contains(".otf") || file.Contains(".ttf"))
            {
                index++;
                continue;
            }

            EditorUtility.DisplayProgressBar("�Ѽ���Դ...", "", index / total);
            var filePath = file.Replace('\\', '/');
            var m_assetInfo = new AssetInfo(filePath);
            assetsList.Add(m_assetInfo);
            index++;
        }

        EditorUtility.ClearProgressBar();
        CreatAllGroups(assetsList);
        MoveFirstAssetToLocalGroup();
        EditorUtility.DisplayDialog("��ʾ", string.Format("����Group�������,��Դ����{0}", assetsList.Count), "ȷ��");
    }

    static void CreatAllGroups(List<AssetInfo> list_)
    {
        var total = list_.Count;
        var index = 1f;

        foreach (var item in list_)
        {
            EditorUtility.DisplayProgressBar("����Group", item.FileName, index / total);
            var group = GetGroup(item.GroupName);
            string guid = AssetDatabase.AssetPathToGUID(item.FilePath);

            AddressableAssetEntry entry = setting.CreateOrMoveEntry(guid, group, false, true);
            entry.address = item.FileName;
            entry.SetLabel(item.LabelName, true, true, false);
            index++;
        }

        EditorUtility.ClearProgressBar();
    }

    static AddressableAssetGroup GetGroup(string groupName_)
    {
        var schemas = setting.DefaultGroup.Schemas[0];
        AddressableAssetGroup group = setting.FindGroup(groupName_);
        if (group != null) return group;
        return setting.CreateGroup(groupName_, false, false, false, new List<AddressableAssetGroupSchema> { setting.DefaultGroup.Schemas[0], setting.DefaultGroup.Schemas[1] });
    }

    static void MoveFirstAssetToLocalGroup()
    {
        var total = EditorPath.FirstAssetsPaths.Count;
        var index = 1f;
        foreach (var item in EditorPath.FirstAssetsPaths)
        {
            EditorUtility.DisplayProgressBar("�ƶ��װ���Դ...", "", index / total);
            var group = GetGroup(EditorPath.LocalFistGroup);
            string guid = AssetDatabase.AssetPathToGUID(item);
            Debug.Log(item);
            AddressableAssetEntry entry = setting.CreateOrMoveEntry(guid, group, false, true);
            entry.address = Path.GetFileNameWithoutExtension(item);
            entry.SetLabel(EditorPath.LocalFistGroup, true, true, false);
            index++;
        }
        EditorUtility.ClearProgressBar();
    }
}

