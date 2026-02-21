using SymphonyFrameWork.System;
using System.Linq;
using UnityEngine;

namespace TestNameSpace
{
    public class MultiSceneLoad : MonoBehaviour
    {
        [SerializeField]
        private SceneListEnum[] _sceneListEnums;

        void Start()
        {
            string[] scenes = _sceneListEnums.Select(s => s.ToString()).ToArray();
            SceneLoader.LoadScenes(scenes, loadingProgress =>
            {
                Debug.Log($"Loading Progress: {loadingProgress * 100}%");
            }).ContinueWith(task =>
            {
                if (task.Result)
                {
                    Debug.Log("All scenes loaded successfully.");
                }
                else
                {
                    Debug.LogError("Failed to load scenes.");
                }
            });
        }

    }
}
