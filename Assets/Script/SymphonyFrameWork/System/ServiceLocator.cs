using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// シングルトンのインスタンスを統括して管理するクラス
    /// </summary>
    //インスタンスを一時的にシーンロードから切り離したい時にも使用できる
    public static class ServiceLocator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _instance = null;
            _singletonObjects.Clear();
        }

        [Tooltip("シングルトン化するインスタンスのコンテナ")]
        private static GameObject _instance;
        [Tooltip("シングルトン登録されている型のインスタンス辞書")]
        private static Dictionary<Type, Component> _singletonObjects = new();

        /// <summary>
        /// インスタンスコンテナが無い場合に生成する
        /// </summary>
        private static void CreateInstance()
        {
            if (_instance is not null)
            {
                return;
            }

            GameObject instance = new GameObject("ServiceLocator");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }

        /// <summary>
        /// 入れられたコンポーネントをロケーターに登録する
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

            Debug.Log($"{typeof(T).Name}クラスの{instance.name}が" +
                $"{type switch { LocateType.Locator => "ロケート", LocateType.Singleton => "シングルトン", _ => string.Empty }}登録されました");

            if (type == LocateType.Singleton)
            {
                CreateInstance();
                instance.transform.SetParent(_instance.transform);
            }
        }

        /// <summary>
        /// 指定したインスタンスを破棄する
        /// </summary>
        /// <typeparam name="T">破棄したいインスタンスの型</typeparam>
        public static void DestroyInstance<T>(T instance) where T : Component
        {
            //インスタンスが登録されたコンポーネントか
            if (_singletonObjects.TryGetValue(typeof(T), out Component md) && md == instance)
            {
                DestroyInstance<T>();
            }
        }

        /// <summary>
        /// 指定した型のインスタンスを破棄する
        /// </summary>
        /// <typeparam name="T">破棄したいインスタンスの型</typeparam>
        public static void DestroyInstance<T>() where T : Component
        {
            if (_singletonObjects.TryGetValue(typeof(T), out Component md))
            {
                Object.Destroy(md.gameObject);
                _singletonObjects.Remove(typeof(T));
                Debug.Log($"{typeof(T).Name}が破棄されました");
            }
            else
            {
                Debug.Log($"{typeof(T).Name}は登録されていません");
            }
        }

        /// <summary>
        /// 登録されたインスタンスを返す
        /// </summary>
        /// <typeparam name="T">取得したいインスタンスの型</typeparam>
        /// <returns>指定した型のインスタンス</returns>
        public static T GetInstance<T>() where T : Component
        {
            if (_singletonObjects.TryGetValue(typeof(T), out Component md))
            {
                if (md != null)
                {
                    return md as T;
                }
                else
                {
                    Debug.LogError($"{typeof(T).Name} は破棄されています。");
                    return null;
                }
            }

            Debug.LogWarning($"{typeof(T).Name} は登録されていません。");
            return null;
        }

        public enum LocateType
        {
            Singleton,
            Locator,
        }
    }
}