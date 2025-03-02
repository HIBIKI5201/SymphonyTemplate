using SymphonyFrameWork.Core;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace SymphonyFrameWork.Config
{
    public static class SymphonyConfigLocator
    {

        /// <summary>
        ///     それぞれのパスを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static (string path, string filePath)? GetFullPath<T>() where T : ScriptableObject
        {
            //ファイル名を生成
            var filePath = $"{typeof(T).Name}.asset";

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("file path is null or empty.");
                return null;
            }

            return ($"{SymphonyConstant.RESOURCES_RUNTIME_PATH}/", filePath);
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

            return Resources.Load<T>(typeof(T).Name);
        }
    }
}