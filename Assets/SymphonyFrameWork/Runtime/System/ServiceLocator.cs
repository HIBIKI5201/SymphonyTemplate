using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    ///     インスタンスを一時的にシーンロードから切り離したい時にも使用できる
    /// </summary>
    public static class ServiceLocator
    {
        public enum LocateType
        {
            Singleton,
            Locator
        }

        public static GameObject Instance
        {
            private set => _instance = value;
            get
            {
                if (!_instance)
                {
                    var instance = new GameObject("ServiceLocator");

                    SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
                    _instance = instance;
                }
                
                return _instance;
            }
        }
        [Tooltip("シングルトン化するインスタンスのコンテナ")] private static GameObject _instance;

        [Tooltip("シングルトン登録されている型のインスタンス辞書")]
        private static readonly Dictionary<Type, Component> _singletonObjects = new();
        
        [Tooltip("シングルトン登録まで待機してから実行されるイベント")]
        private static readonly Dictionary<Type, Action> _waitingActions = new();
        [Tooltip("シングルトン登録まで待機してから実行されるイベントのインスタンスを返すイベント")]
        private static readonly Dictionary<Type, Delegate> _waitingActionsWithInstance = new();

        internal static void Initialize()
        {
            _instance = null;
            _singletonObjects.Clear();
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
            //ログを出力
            if (EditorPrefs.GetBool(EditorSymphonyConstant.ServiceLocatorSetInstanceKey,
                EditorSymphonyConstant.ServiceLocatorSetInstanceDefault))
                Debug.Log($"{typeof(T).Name}クラスの{instance.name}が" +
                    $"{type switch { LocateType.Locator => "ロケート", LocateType.Singleton => "シングルトン", _ => string.Empty }}登録されました");
#endif

            #region 待機中のイベントを発火
            if (_waitingActions.TryGetValue(typeof(T), out var waitingAction))
            {
                waitingAction?.Invoke();
                _waitingActions.Remove(typeof(T));
            }

            if (_waitingActionsWithInstance.TryGetValue(typeof(T), out var del))
            {
                if (del is Action<T> waitingActionWithInstance)
                {
                    waitingActionWithInstance.Invoke(instance);
                }

                _waitingActionsWithInstance.Remove(typeof(T));
            }

            #endregion

            if (type == LocateType.Singleton) //もしシングルトンなら親をLocatorに設定
            {
                instance.transform.SetParent(Instance.transform);
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
                //ログを出力
                if (EditorPrefs.GetBool(EditorSymphonyConstant.ServiceLocatorDestroyInstanceKey,
                    EditorSymphonyConstant.ServiceLocatorDestroyInstanceDefault))
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
            //ログを出力
            if (EditorPrefs.GetBool(EditorSymphonyConstant.ServiceLocatorGetInstanceKey,
                EditorSymphonyConstant.ServiceLocatorGetInstanceDefault))
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
                if (EditorPrefs.GetBool(EditorSymphonyConstant.ServiceLocatorGetInstanceKey,
                    EditorSymphonyConstant.ServiceLocatorGetInstanceDefault))
                {
                    SymphonyDebugLog.AddText(text);
                    SymphonyDebugLog.TextLog(kind);
                }
#endif
            }
        }

        /// <summary>
        ///     オブジェクトが登録された時に実行される
        /// </summary>
        /// <param name="action"></param>
        public static void RegisterAfterLocate<T>(Action action) where T : Component
        {
            //既にロードされていたら終了
            if (_singletonObjects.ContainsKey(typeof(T)))
            {
                action?.Invoke();
                return;
            }

            if (!_waitingActions.TryAdd(typeof(T), action))
            {
                _waitingActions[typeof(T)] += action;
            }
        }

        /// <summary>
        ///     オブジェクトが登録された時に実行される
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void RegisterAfterLocate<T>(Action<T> action) where T : Component
        {
            //既にロードされていたら終了
            if (_singletonObjects.TryGetValue(typeof(T), out var instance))
            {
                action?.Invoke((T)instance);
                return;
            }

            if (_waitingActionsWithInstance.TryGetValue(typeof(T), out var existing))
            {
                _waitingActionsWithInstance[typeof(T)] = Delegate.Combine(existing, action);
            }
            else
            {
                _waitingActionsWithInstance[typeof(T)] = action;
            }
        }

        /// <summary>
        ///     インスタンスが返されるまで待機する
        /// </summary>
        /// <param name="grace">最大待機フレーム数</param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> GetInstanceAsync<T>(byte grace = 120, CancellationToken token = default) where T : Component
        {
            float time = Time.time;
            
            while (grace + time > Time.time)
            {
                T result = GetInstance<T>();
                
                if (result) //nullでなければ返して終了
                    return result;
                
                await Awaitable.NextFrameAsync(token);
            }

            return null;
        }
    }
}