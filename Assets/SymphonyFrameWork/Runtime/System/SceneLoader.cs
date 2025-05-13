using System;
using System.Collections.Generic;
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
        private static readonly HashSet<string> _loadingSceneList = new();

        internal static void Initialize()
        {
            _sceneDict.Clear();

            var config = SymphonyConfigLocator.GetConfig<SceneManagerConfig>();
            if (config)
                foreach (var scene in config.InitializeSceneList)
                    _ = LoadScene(scene.ToString());
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
        /// <returns>ロードに成功したか</returns>
        public static async Task<bool> LoadScene(string sceneName,
            Action<float> loadingAction = null,
            LoadSceneMode mode = LoadSceneMode.Additive)
        {
            //ロードしようとしているシーンが既にないか確認
            if (_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}シーンは既にロードされています");
                return false;
            }

            //シーンのロードを開始
            var operation = SceneManager.LoadSceneAsync(sceneName, mode);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}シーンは登録されていません");
                return false;
            }
            
            _loadingSceneList.Add(sceneName);
            
            //ロード中の処理
            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync();
            }

            //シングルロードの場合は辞書をクリアする
            if (mode == LoadSceneMode.Single)
            {
                _sceneDict.Clear();
            }
            
            _loadingSceneList.Remove(sceneName);

            //辞書にシーン名とシーン情報を保存
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                _sceneDict.TryAdd(sceneName, loadedScene);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     シーンをアンロードする
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="loadingAction">ロードの進捗率を引数にしたメソッド</param>
        /// <returns>アンロードに成功したか</returns>
        public static async Task<bool> UnloadScene(string sceneName, Action<float> loadingAction = null)
        {
            if (!_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}シーンはロードされていません");
                return false;
            }

            var operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation == null)
            {
                Debug.LogError($"{sceneName}シーンは登録されていません");
                return false;
            }

            while (!operation.isDone)
            {
                loadingAction?.Invoke(operation.progress);
                await Awaitable.NextFrameAsync();
            }

            _sceneDict.Remove(sceneName);

            return true;
        }

        /// <summary>
        ///     指定したシーンがロードされるまで待機する
        /// </summary>
        /// <param name="sceneName"></param>
        public static async Task WaitForLoadScene(string sceneName)
        {
            while (_loadingSceneList.Contains(sceneName))
            {
                await Awaitable.NextFrameAsync();
            }
        }
    }
}