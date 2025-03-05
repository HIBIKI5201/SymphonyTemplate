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
            FileCheck<SceneManagerConfig>(ConfigType.Runtime);
            FileCheck<AudioManagerConfig>(ConfigType.Runtime);
            FileCheck<AutoEnumGeneratorConfig>(ConfigType.Editor);
        }

        /// <summary>
        ///     ファイルが存在するか確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void FileCheck<T>(ConfigType type) where T : ScriptableObject
        {
            var paths = type switch
            {
                ConfigType.Runtime => SymphonyConfigLocator.GetFullPath<T>(),
                ConfigType.Editor => SymphonyEditorConfigLocator.GetFullPath<T>(),
                _ => null
            };
            if (paths == null)
            {
                Debug.LogWarning(typeof(T).Name + " doesn't exist!");
                return;
            }

            var (path, filePath) = paths.Value;

            // ファイルが存在するなら終了
            if (AssetDatabase.LoadAssetAtPath<T>(path + filePath) != null) return;

            // リソースフォルダがなければ生成
            CreateResourcesFolder(path);

            // 対象のアセットを生成してResources内に配置
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path + filePath);

            // 変更を反映させるためにRefresh()を呼び出す
            AssetDatabase.Refresh();

            // アセットを保存
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

        private enum ConfigType : int
        {
            Runtime,
            Editor,
        }
    }
}