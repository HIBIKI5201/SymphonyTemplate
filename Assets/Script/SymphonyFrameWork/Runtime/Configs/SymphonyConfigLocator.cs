using System;
using System.Collections.Generic;
using SymphonyFrameWork.Core;
using SymphonyFrameWork.Editor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SymphonyFrameWork.Config
{
    public static class SymphonyConfigLocator
    {
        //各クラスのファイルタイプ
        private static readonly Dictionary<Type, PathType> _typeDict = new()
        {
            { typeof(SceneManagerConfig), PathType.Runtime },
            { typeof(AutoEnumGeneratorConfig), PathType.Editor }
        };

        /// <summary>
        ///     それぞれのパスを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (string path, string filePath)? GetFullPath<T>() where T : ScriptableObject
        {
            //パスを取得
            if (!_typeDict.TryGetValue(typeof(T), out var type)) return null;

            var path = type switch
            {
                PathType.Runtime => SymphonyConstant.RESOURCES_RUNTIME_PATH,
                PathType.Editor => SymphonyConstant.RESOURCES_EDITOR_PATH,
                _ => string.Empty
            } + "/";

            if (string.IsNullOrEmpty(path)) return null;

            //ファイルパスを生成
            var filePath = $"{typeof(T).Name}.asset";

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("file path is null or empty.");
                return null;
            }

            return (path, filePath);
        }

        /// <summary>
        ///     指定した型のアセットを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetConfig<T>() where T : ScriptableObject
        {
            var paths = GetFullPath<T>();
            if (paths == null) return null;

            if (_typeDict.TryGetValue(typeof(T), out var type))
            {
                if (type == PathType.Runtime) return Resources.Load<T>(typeof(T).Name);
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<T>(paths.Value.path + paths.Value.filePath);
#else
                    return null;
#endif
            }

            return null;
        }

        private enum PathType
        {
            Runtime,
            Editor
        }
    }
}