using UnityEngine;

namespace SymphonyFrameWork.Config
{
    /// <summary>
    /// シーンマネージャーのコンフィグを格納する
    /// </summary>
    public class SceneManagerConfig : ScriptableObject
    {
        [SerializeField, Tooltip("ロードシーンを有効化するかどうか")]
        private bool _isActiveLoadScene;
        public bool IsActiveLoadScene { get => _isActiveLoadScene; }

        [SerializeField, Tooltip("ロード中に表示されるシーン")]
        private string _loadScene;
        public string LoadScene { get => _loadScene; }
    }
}
