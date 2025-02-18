using SymphonyFrameWork.Config;
using SymphonyFrameWork.Core;
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
        /// <summary>
        /// ファイルがあるか確認する
        /// </summary>
        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>(FileType.Runtime);
            FileCheck<SymphonyWindowConfig>(FileType.Editor);
        }

        /// <summary>
        /// ファイルがない場合は再生成する
        /// </summary>
        /// <param name="type"></param>
        /// <typeparam name="T"></typeparam>
        private static void FileCheck<T>(FileType type) where T : ScriptableObject
        {
            //パスを取得
            string path = type switch
            {
                FileType.Runtime => SymphonyConstant.RESOURCES_RUNTIME_PATH,
                FileType.Editor => SymphonyConstant.RESOURCES_EDITOR_PATH,
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("SymphonyConfigManager: File not found: " + type.ToString());
                return;
            }
            
            //型の名前でパスを指定
            string filePath = $"{path}/{typeof(T).Name}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<T>(filePath) != null) return;
            
            //フォルダがなければ生成
            CreateResourcesFolder(path);

            //対象のアセットを生成してResources内に配置
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, filePath);
            AssetDatabase.SaveAssets();

            SymphonyDebugLog.DirectLog($"'{path}' に新しい {typeof(T).Name} を作成しました。");
        }

        /// <summary>
        /// リソースフォルダが無ければ生成
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

        private enum FileType
        {
            Runtime,
            Editor,
        }
    }
}
