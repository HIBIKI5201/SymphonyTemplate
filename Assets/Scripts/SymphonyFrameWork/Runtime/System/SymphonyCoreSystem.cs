using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.System
{
    /// <summary>
    ///     SymphonyFrameWorkの管理シーンを持つ
    /// </summary>
    public static class SymphonyCoreSystem
    {
        private static Scene? _systemScene;

        /// <summary>
        ///     初期化でシステム用のシーンを作成
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameBeforeSceneLoaded()
        {
            //専用のシーン生成
            _systemScene = SceneManager.CreateScene("SymphonySystem");

            //各クラスの初期化
            PauseManager.Initialize();
            ServiceLocator.Initialize();
            SceneLoader.Initialize();
            AudioManager.Initialize();
        }

        public static async void MoveObjectToSymphonySystem(GameObject go)
        {
            //シーンが制作されているか、対象がnullになったら進む
            await SymphonyTask.WaitUntil(() => _systemScene != null || go == null);

            if (go) SceneManager.MoveGameObjectToScene(go, _systemScene.Value);
        }
    }
}