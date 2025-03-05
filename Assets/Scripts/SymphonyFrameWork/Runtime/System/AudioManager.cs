using UnityEngine;

namespace SymphonyFrameWork.System
{
    /// <summary>
    /// オーディオ再生
    /// </summary>
    public static class AudioManager
    {
        private static GameObject _instance;

        internal static void Initialize()
        {
            _instance = null;
        }

        private static void CreateInstance()
        {
            if (_instance is not null) return;

            var instance = new GameObject("AudioManager");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }
    }
}
