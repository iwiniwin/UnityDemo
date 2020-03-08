using UnityEditor;
using UnityEngine;
using System.IO;
using UKit.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static UKit.Utils.Output;

public class AssetsMenuExtend
{
    /// <summary>
    /// 清除无用资源
    /// </summary>
    [MenuItem("Assets/Extend/Clear")]
    static void UnloadUnusedAssetsImmediate(){
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    /// <summary>
    /// 构建 AssetBundle
    /// </summary>
    [MenuItem("Assets/Extend/Build AssetBundle")]
    static void BuildAssetBundle(){
        string outPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if(Directory.Exists(outPath)){
            Directory.Delete(outPath);
        }
        Directory.CreateDirectory(outPath);

        // 构建AssetBundle
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        // 生成描述文件
        BundleList bundleList = ScriptableObject.CreateInstance<BundleList>();
        foreach (var item in builds)
        {
            foreach (var res in item.assetNames)
            {
                bundleList.bundleDatas.Add(new BundleList.BundleData(){resPath = res, bundlePath = item.assetBundleName});
            }
        }
        AssetDatabase.CreateAsset(bundleList, "Assets/Resources/bundleList.asset");
        
        // 刷新
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 查找重复资源
    /// </summary>
    [MenuItem("Assets/Extend/Find Duplicate Resources")]
    static void FindDuplicateResources(){
        Dictionary<string, string> md5dic = new Dictionary<string, string>();
        string[] paths = AssetDatabase.FindAssets("t:prefab", new string[]{"Assets/Resources"});
        foreach (var prefabGUID in paths)
        {
            string prefabAssetPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            string[] dependencies = AssetDatabase.GetDependencies(prefabAssetPath, true);
            for (int i = 0; i < dependencies.Length; i++)
            {
                string assetPath = dependencies[i];
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                // 满足贴图和模型资源
                if(importer is TextureImporter || importer is ModelImporter || true){
                    string md5 = FileSystem.GetMD5Hash(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
                    string path;
                    if(!md5dic.TryGetValue(md5, out path)){
                        md5dic[md5] = assetPath;
                    }else{
                        if(path != assetPath){
                            Debug.LogFormat("{0} {1} 资源发生重复！", path, assetPath);
                        }
                    }
                }
            }
        }
    }

    
    static void Find(){
        Dictionary<string, string> guidDics = new Dictionary<string, string>();
        foreach (var o in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if(!string.IsNullOrEmpty(path)){
                string guid = AssetDatabase.AssetPathToGUID(path);
                if(!guidDics.ContainsKey(guid)){
                    guidDics[guid] = o.name;
                }
            }
        }
        if(guidDics.Count > 0){
            List<string> withoutExtensions = new List<string>(){".prefab", ".unity", ".mat", ".asset"};
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
                .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if(i % 20 == 0){
                    bool isCancel = EditorUtility.DisplayCancelableProgressBar("", file, (float)i / (float)files.Length);
                    if(isCancel){
                        break;
                    }
                }
                foreach (KeyValuePair<string, string> guidItem in guidDics)
                {
                    if(Regex.IsMatch(File.ReadAllText(file), guidItem.Key)){
                        Debug.Log(string.Format("name: {0} file: {1}", guidItem.Value, file), AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }

    static string GetRelativeAssetsPath(string path){
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace("\\", "/");
    }

    [MenuItem("Assets/Extend/Find References", true)]  // true 验证函数，在调用具有相同 item name 的菜单函数之前调用
    static bool VFind(){
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }
}
