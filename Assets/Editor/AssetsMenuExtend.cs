using UnityEditor;
using UnityEngine;
using System.IO;

public class UnloadUnusedAssets
{
    /// <summary>
    /// 清除无用资源
    /// </summary>
    [MenuItem("Assets/Extend/Clear")]
    static void UnloadUnusedAssetsImmediate(){
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    [MenuItem("Assets/Extend/BuildAssetBundle")]
    static void BuildAssetBundle(){
        string outPath = Path.Combine(Application.dataPath, "StreamingAssets");
        if(Directory.Exists(outPath)){
            Directory.Delete(outPath);
        }
        Directory.CreateDirectory(outPath);

        // 构建AssetBundle
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        
        // 刷新
        AssetDatabase.Refresh();
    }
}
