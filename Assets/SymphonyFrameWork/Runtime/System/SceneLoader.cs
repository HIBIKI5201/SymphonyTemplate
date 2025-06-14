﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SymphonyFrameWork.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.System
{
    /// <summary>
    ///     シーンのロードを管理するクラス
    /// </summary>
    public static class SceneLoader
    {
        private static readonly Dictionary<string, Scene> _sceneDict = new();
        private static readonly Dictionary<string, Action> _loadingSceneDict = new();

        internal static void Initialize()
        {
            _sceneDict.Clear();

            var config = SymphonyConfigLocator.GetConfig<SceneManagerConfig>();
            if (config)
                foreach (var scene in config.InitializeSceneList)
                    _ = LoadScene(scene);
        }

        /// <summary>
        ///     ゲーム開始時の初期化処理
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            var scene = SceneManager.GetActiveScene();
            _sceneDict.Add(scene.name, scene);
        }

        /// <summary>
        ///     ロードされているシーンを返す
        ///     ない場合はnullを返す
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="scene"></param>
        /// <returns></returns>
        public static bool GetExistScene(string sceneName, out Scene scene)
        {
            return _sceneDict.TryGetValue(sceneName, out scene);
        }

        /// <summary>
        ///     シーンをアクティブにする
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool SetActiveScene(string sceneName)
        {
            if (_sceneDict.TryGetValue(sceneName, out var scene))
            {
                SceneManager.SetActiveScene(scene);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     シーンをロードする
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="loadingAction">ロードの進捗率を引数にしたメソッド</param>
        /// <param name="mode"></param>
        /// <param name="token"></param>
        /// <returns>ロードに成功したか</returns>
        public static async Task<bool> LoadScene(
            string sceneName,
            Action<float> loadingAction = null,
            LoadSceneMode mode = LoadSceneMode.Additive,
            CancellationToken token = default)
        {
            //ロードしようとしているシーンが既にないか確認
            if (_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}シーンは既にロードされています");
                return false;
            }

            //ロード開始
            var operation = SceneManager.LoadSceneAsync(sceneName, mode);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}シーンは登録されていません");
                return false;
            }

            _loadingSceneDict.Add(sceneName, null);

            //ロード中
            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync(token);
            }

            //シングルロードの場合は辞書をクリアする
            if (mode == LoadSceneMode.Single)
            {
                _sceneDict.Clear();
            }

            //ロード終了後にロード待ちしていたイベントを実行
            if (_loadingSceneDict.TryGetValue(sceneName, out var action))
            {
                action?.Invoke();
                _loadingSceneDict.Remove(sceneName);
            }

            //辞書にシーン名とシーン情報を保存
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            var isLoadSuccess = loadedScene.IsValid() && loadedScene.isLoaded;
            if (isLoadSuccess)
            {
                _sceneDict.TryAdd(sceneName, loadedScene);
            }

            return loadedScene.IsValid() && loadedScene.isLoaded;
        }

        /// <summary>
        ///     シーンをアンロードする
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="loadingAction">ロードの進捗率を引数にしたメソッド</param>
        /// <param name="token"></param>
        /// <returns>アンロードに成功したか</returns>
        public static async Task<bool> UnloadScene(
            string sceneName,
            Action<float> loadingAction = null,
            CancellationToken token = default)
        {
            if (!_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}シーンはロードされていません");
                return false;
            }

            //アンロード開始
            var operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}シーンは登録されていません");
                return false;
            }

            //ロード中
            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync(token);
            }

            _sceneDict.Remove(sceneName);

            return true;
        }

        /// <summary>
        ///     シーンがロードされた時に実行される
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        public static void RegisterAfterSceneLoad(string sceneName, Action action)
        {
            //既にロードされていたら終了
            if (_sceneDict.ContainsKey(sceneName))
            {
                action?.Invoke();
                return;
            }

            //アクションを追加
            if (!_loadingSceneDict.TryAdd(sceneName, action))
                _loadingSceneDict[sceneName] += action;
        }

        /// <summary>
        ///     指定したシーンがロードされるまで待機する
        /// </summary>
        /// <param name="sceneName"></param>
        public static async Task WaitForLoadSceneAsync(string sceneName)
        {
            while (!_sceneDict.ContainsKey(sceneName))
            {
                await Awaitable.NextFrameAsync();
            }
        }

        /// <summary>
        ///     指定したシーンがロードされるまで待機する
        /// </summary>
        /// <param name="sceneName"></param>
        [Obsolete]
        public static async Task WaitForLoadScene(string sceneName)
        {
            while (!_sceneDict.ContainsKey(sceneName))
            {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}