using SymphonyFrameWork.Debugger;
using UnityEngine;
using System.IO;
using SymphonyFrameWork.Config;
using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// コンフィグ用データを生成するクラス
    /// </summary>
    [InitializeOnLoad]
    public static class SymphonyConfigManager
    {
        private const string FrameWork_Path = "Assets/Script/SymphonyFrameWork/Resources";

        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>();
        }

        private static void FileCheck<T>() where T : ScriptableObject
        {
            //型の名前でパスを指定
            string filePath = $"{FrameWork_Path}/{typeof(T).Name}.asset";

            // ファイルが存在しない場合
            if (AssetDatabase.LoadAssetAtPath<T>(filePath) == null)
            {
                CreateResourcesFolder(); 

                //対象のアセットを生成してResources内に配置
                T asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, filePath);
                AssetDatabase.SaveAssets();

                SymphonyDebugLog.DirectLog($"'{filePath}' に新しい {nameof(T)} を作成しました。");
            }
        }

        /// <summary>
        /// リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder()
        {
            string resourcesPath = FrameWork_Path;

            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
