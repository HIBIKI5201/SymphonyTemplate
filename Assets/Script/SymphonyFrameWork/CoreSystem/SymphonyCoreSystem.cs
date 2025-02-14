using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    public static class SymphonyCoreSystem
    {
        private static Scene? _systemScene;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameBeforeSceneLoaded()
        {
            _systemScene = SceneManager.CreateScene("SymphonySystem");
        }

        public static async void MoveObjectToSymphonySystem(GameObject go)
        {
            await SymphonyTask.WaitUntil(() => _systemScene != null, default);

            SceneManager.MoveGameObjectToScene(go, _systemScene.Value);
        }
    }
}
