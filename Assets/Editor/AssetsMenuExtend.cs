using UnityEditor;
using UnityEngine;
using System.IO;
using UKit.Core;
using System.Collections.Generic;
using static UKit.Utils.Output;

public class UnloadUnusedAssets
{
    /// <summary>
    /// 清除无用资源
    /// </summary>
    [MenuItem("Assets/Extend/Clear")]
    static void UnloadUnusedAssetsImmediate(){
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

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
}
