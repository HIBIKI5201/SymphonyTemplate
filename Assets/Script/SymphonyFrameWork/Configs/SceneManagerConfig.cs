using UnityEngine;
using SymphonyFrameWork.Attribute;
using UnityEditor;
using System.Linq;
using System.IO;

namespace SymphonyFrameWork.Config
{
    /// <summary>
    /// シーンマネージャーのコンフィグを格納する
    /// </summary>
    public class SceneManagerConfig : ScriptableObject
    {
        [DisplayText("開発中の機能です")]

        [Space]

        [SerializeField, Tooltip("ロードシーンを有効化するかどうか")]
        private bool _isActiveLoadScene;
        public bool IsActiveLoadScene { get => _isActiveLoadScene; }

        [SerializeField, Tooltip("ロード中に表示されるシーン")]
        private string _loadScene;
        public string LoadScene { get => _loadScene; }

        [ReadOnly, SerializeField, Tooltip("Enumを生成するシーンの一覧")]
        private string[] _sceneList = new string[] { };
        public string[] SceneList { get => _sceneList; }

        private void OnEnable()
        {
            _sceneList = EditorBuildSettings.scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();
        }
    }
}
