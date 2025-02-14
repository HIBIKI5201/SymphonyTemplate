using SymphonyFrameWork.Utility;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// SymphonyFrameWork�̊Ǘ��V�[��������
    /// </summary>
    public static class SymphonyCoreSystem
    {
        private static Scene? _systemScene;

        /// <summary>
        /// �������ŃV�X�e���p�̃V�[�����쐬
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameBeforeSceneLoaded()
        {
            _systemScene = SceneManager.CreateScene("SymphonySystem");
        }

        public static async void MoveObjectToSymphonySystem(GameObject go)
        {
            //�V�[�������삳��Ă��邩�A�Ώۂ�null�ɂȂ�����i��
            await SymphonyTask.WaitUntil(() => _systemScene != null || go == null);

            if (go)
            {
                SceneManager.MoveGameObjectToScene(go, _systemScene.Value);
            }
        }
    }
}
