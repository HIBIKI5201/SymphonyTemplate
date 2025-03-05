using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using SymphonyFrameWork.Debugger;
using SymphonyFrameWork.Core;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SymphonyFrameWork.System
{
    /// <summary>
    ///     シングルトンのインスタンスを統括して管理するクラス
    /// </summary>
    //インスタンスを一時的にシーンロードから切り離したい時にも使用できる
    public static class ServiceLocator
    {
        public enum LocateType
        {
            Singleton,
            Locator
        }

        [Tooltip("シングルトン化するインスタンスのコンテナ")] private static GameObject _instance;

        [Tooltip("シングルトン登録されている型のインスタンス辞書")]
        private static readonly Dictionary<Type, Component> _singletonObjects = new();

        internal static void Initialize()
        {
            _instance = null;
            _singletonObjects.Clear();
        }

        /// <summary>
        ///     インスタンスコンテナが無い場合に生成する
        /// </summary>
        private static void CreateInstance()
        {
            if (_instance is not null) return;

            var instance = new GameObject("ServiceLocator");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }

        /// <summary>
        ///     入れられたコンポーネントをロケーターに登録する
        /// </summary>
        /// <typeparam name="T">登録する型</typeparam>
        /// <param name="instance">インスタンス</param>
        /// <returns>登録が成功したらtrue、失敗したらfalse</returns>
        public static void SetInstance<T>(T instance, LocateType type = LocateType.Locator) where T : Component
        {
            // 既に登録されている場合は追加できない
            if (!_singletonObjects.TryAdd(typeof(T), instance))
            {
                Object.Destroy(instance.gameObject);
                return;
            }

#if UNITY_EDITOR
            if (EditorPrefs.GetBool(SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorSetInstanceKey,
                SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorSetInstanceDefault))
                Debug.Log($"{typeof(T).Name}クラスの{instance.name}が" +
                    $"{type switch { LocateType.Locator => "ロケート", LocateType.Singleton => "シングルトン", _ => string.Empty }}登録されました");
#endif

            if (type == LocateType.Singleton)
            {
                CreateInstance();
                instance.transform.SetParent(_instance.transform);
            }
        }

        /// <summary>
        ///     指定したインスタンスを破棄する
        /// </summary>
        /// <typeparam name="T">破棄したいインスタンスの型</typeparam>
        public static void DestroyInstance<T>(T instance) where T : Component
        {
            //インスタンスが登録されたコンポーネントか
            if (_singletonObjects.TryGetValue(typeof(T), out var md) && md == instance) DestroyInstance<T>();
        }

        /// <summary>
        ///     指定した型のインスタンスを破棄する
        /// </summary>
        /// <typeparam name="T">破棄したいインスタンスの型</typeparam>
        public static void DestroyInstance<T>() where T : Component
        {
            if (_singletonObjects.TryGetValue(typeof(T), out var md))
            {
                Object.Destroy(md.gameObject);
                _singletonObjects.Remove(typeof(T));

#if UNITY_EDITOR
                if (EditorPrefs.GetBool(SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorDestroyInstanceKey,
                    SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorDestroyInstanceDefault))
                    Debug.Log($"{typeof(T).Name}が破棄されました");
#endif

            }
            else
            {
                Debug.LogWarning($"{typeof(T).Name}は登録されていません");
            }
        }

        /// <summary>
        ///     登録されたインスタンスを返す
        /// </summary>
        /// <typeparam name="T">取得したいインスタンスの型</typeparam>
        /// <returns>指定した型のインスタンス</returns>
        public static T GetInstance<T>() where T : Component
        {
#if UNITY_EDITOR
            if (EditorPrefs.GetBool(SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorGetInstanceKey,
                SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorGetInstanceDefault))
                SymphonyDebugLog.AddText($"ServiceLocator\n{typeof(T).Name}の取得がリクエストされました。");
#endif

            if (_singletonObjects.TryGetValue(typeof(T), out var md))
            {
                if (md)
                {
                    OutputLog($"正常に行われました。");
                    return md as T;
                }

                OutputLog($"{typeof(T).Name} は破棄されています。", SymphonyDebugLog.LogKind.Warning);
                return null;
            }

            OutputLog($"{typeof(T).Name} は登録されていません。", SymphonyDebugLog.LogKind.Warning);
            return null;

            void OutputLog(string text, SymphonyDebugLog.LogKind kind = SymphonyDebugLog.LogKind.Normal)
            {
#if UNITY_EDITOR
                if (EditorPrefs.GetBool(SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorGetInstanceKey,
                    SymphonyConstant.EditorSymphonyConstrant.ServiceLocatorGetInstanceDefault))
                {
                    SymphonyDebugLog.AddText(text);
                    SymphonyDebugLog.TextLog(kind);
                }
#endif
            }
        }
    }
}