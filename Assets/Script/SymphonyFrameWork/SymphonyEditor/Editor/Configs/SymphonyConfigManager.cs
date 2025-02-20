using System.IO;
using SymphonyFrameWork.Config;
using SymphonyFrameWork.Debugger;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    ///     コンフィグ用データを生成するクラス
    /// </summary>
    [InitializeOnLoad]
    public static class SymphonyConfigManager
    {
        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>();
            FileCheck<AutoEnumGeneratorConfig>();
        }

        /// <summary>
        ///     ファイルが存在するか確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void FileCheck<T>() where T : ScriptableObject
        {
            var paths = SymphonyConfigLocator.GetFullPath<T>();
            if (paths == null)
            {
                Debug.LogWarning(typeof(T).Name + " doesn't exist!");
                return;
            }

            var (path, filePath) = paths.Value;

            //ファイルが存在するなら終了
            if (AssetDatabase.LoadAssetAtPath<T>(path + filePath) != null) return;

            //フォルダがなければ生成
            CreateResourcesFolder(path);

            //対象のアセットを生成してResources内に配置
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path + filePath);
            AssetDatabase.SaveAssets();

            SymphonyDebugLog.DirectLog($"'{path}' に新しい {typeof(T).Name} を作成しました。");
        }

        /// <summary>
        ///     リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder(string resourcesPath)
        {
            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }
        }
    }
}