using SymphonyFrameWork.Config;
using SymphonyFrameWork.Debugger;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// コンフィグ用データを生成するクラス
    /// </summary>
    [InitializeOnLoad]
    public static class SymphonyConfigManager
    {
        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>();
        }

        private static void FileCheck<T>() where T : ScriptableObject
        {
            //型の名前でパスを指定
            string filePath = $"{SymphonyConstant.RESOURCES_PATH}/{typeof(T).Name}.asset";

            // ファイルが存在しない場合
            if (AssetDatabase.LoadAssetAtPath<T>(filePath) == null)
            {
                CreateResourcesFolder();

                //対象のアセットを生成してResources内に配置
                T asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, filePath);
                AssetDatabase.SaveAssets();

                SymphonyDebugLog.DirectLog($"'{SymphonyConstant.RESOURCES_PATH}' に新しい {typeof(T).Name} を作成しました。");
            }
        }

        /// <summary>
        /// リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder()
        {
            string resourcesPath = SymphonyConstant.RESOURCES_PATH;

            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
