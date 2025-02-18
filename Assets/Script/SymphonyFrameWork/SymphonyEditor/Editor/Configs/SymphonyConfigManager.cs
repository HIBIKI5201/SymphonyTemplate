using System;
using System.Collections.Generic;
using SymphonyFrameWork.Config;
using SymphonyFrameWork.Core;
using SymphonyFrameWork.Debugger;
using System.IO;
using System.Linq;
using PlasticGui;
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
        //各クラスのファイルタイプ
        private static  Dictionary<Type, string> _typeDict = new()
        {
            {typeof(SceneManagerConfig), SymphonyConstant.RESOURCES_RUNTIME_PATH},
            {typeof(EnumGeneratorConfig), SymphonyConstant.RESOURCES_EDITOR_PATH}
        };
        
        static SymphonyConfigManager()
        {
            FileCheck<SceneManagerConfig>();
            FileCheck<EnumGeneratorConfig>();
        }

        /// <summary>
        /// ファイルが存在するか確認する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static void FileCheck<T>() where T : ScriptableObject
        {
            var paths = GetFullPath<T>();
            if (paths == null)
            {
                Debug.LogWarning(typeof(T).Name + " doesn't exist!");
                return;
            }

            (string path, string filePath) = paths.Value;
            
            //ファイルが存在するなら終了
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
        /// それぞれのパスを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static (string path, string filePath)? GetFullPath<T>() where T : ScriptableObject
        {
            //パスを取得
            if (!_typeDict.TryGetValue(typeof(T), out var path))
            {
                return null;
            }
            
            //ファイルパスに変換
            var filePath = $"{path}/{typeof(T).Name}.asset";;

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("file path is null or empty.");
                return null;
            }
            
            return (path, filePath);
        }

        /// <summary>
        /// 指定した型のアセットを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetConfig<T>() where T : ScriptableObject
        {
            var paths = GetFullPath<T>();
            if (paths == null) return null;
            
            return AssetDatabase.LoadAssetAtPath<T>(paths.Value.filePath);
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
    }
}
