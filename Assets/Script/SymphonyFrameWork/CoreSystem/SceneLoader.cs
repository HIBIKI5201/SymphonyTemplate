using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// シーンのロードを管理するクラス
    /// </summary>
    public static class SceneLoader
    {
        private static Dictionary<string, Scene> _sceneDict = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad()
        {
            _sceneDict.Clear();
        }

        /// <summary>
        ///ゲーム開始時の初期化処理
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            Scene scene = SceneManager.GetActiveScene();
            _sceneDict.Add(scene.name, scene);
        }

        /// <summary>
        /// ロードされているシーンを返す
        /// ない場合はnullを返す
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool GetExistScene(string sceneName, out Scene scene) =>
            _sceneDict.TryGetValue(sceneName, out scene);

        public static bool SetActiveScene(string sceneName)
        {
            if (_sceneDict.TryGetValue(sceneName, out Scene scene))
            {
                SceneManager.SetActiveScene(scene);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// シーンをロードする
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="loadingAction">ロードの進捗率を引数にしたメソッド</param>
        /// <returns>ロードに成功したか</returns>
        public static async Task<bool> LoadScene(string sceneName, Action<float> loadingAction = null)
        {
            if (_sceneDict.ContainsKey(sceneName))
            {
                Debug.LogWarning($"{sceneName}シーンは既にロードされています");
                return false;
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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

            Scene loadedScene = SceneManager.GetSceneByName(sceneName);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                _sceneDict.TryAdd(sceneName, loadedScene);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// シーンをアンロードする
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
    }
}
