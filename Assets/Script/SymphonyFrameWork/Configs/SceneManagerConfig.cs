using UnityEngine;
using SymphonyFrameWork.Attribute;

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

        [DisplayText("Build ProfileのScene Listに入れたシーン名を入れて下さい")]
        [SerializeField, Tooltip("ロードするシーンの一覧")]
        private string[] _sceneList = new string[]
        {
            "System",
            "Load",
            "Ingame",
            "Outgame"
        };

        public string[] SceneList { get => _sceneList; }
    }
}
