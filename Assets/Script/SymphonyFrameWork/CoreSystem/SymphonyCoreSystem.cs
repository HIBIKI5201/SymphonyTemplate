using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// SymphonyFrameWorkの管理シーンを持つ
    /// </summary>
    public static class SymphonyCoreSystem
    {
        private static Scene? _systemScene;

        /// <summary>
        /// 初期化でシステム用のシーンを作成
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameBeforeSceneLoaded()
        {
            _systemScene = SceneManager.CreateScene("SymphonySystem");
        }

        public static async void MoveObjectToSymphonySystem(GameObject go)
        {
            //シーンが制作されているか、対象がnullになったら進む
            await SymphonyTask.WaitUntil(() => _systemScene != null || go == null);

            if (go)
            {
                SceneManager.MoveGameObjectToScene(go, _systemScene.Value);
            }
        }
    }
}
