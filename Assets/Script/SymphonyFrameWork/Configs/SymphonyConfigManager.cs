using SymphonyFrameWork.Debugger;
using UnityEngine;

# if UNITY_EDITOR
using UnityEditor;
#endif

namespace SymphonyFrameWork.Config
{
    [InitializeOnLoad]
    public static class SymphonyConfigManager
    {
#if UNITY_EDITOR
        private const string FrameWork_Path = "Assets/Script/SymphonyFrameWork/";

        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>();
        }

        private static void FileCheck<T>() where T : ScriptableObject
        {
            string filePath = $"{FrameWork_Path}{typeof(T).Name}.asset";

            // ファイルが存在しない場合
            if (AssetDatabase.LoadAssetAtPath<SceneManagerConfig>(filePath) == null)
            {
                SceneManagerConfig asset = ScriptableObject.CreateInstance<SceneManagerConfig>();
                AssetDatabase.CreateAsset(asset, filePath);
                AssetDatabase.SaveAssets();

                SymphonyDebugLog.DirectLog($"'{filePath}' に新しい {nameof(SceneManagerConfig)} を作成しました。");
            }
        }
    }
#endif
}
