using SymphonyFrameWork.Editor;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Config
{
    public partial class SceneManagerConfig
    {
        [Space]

        [SerializeField]
        private bool _autoListUpdate = true;

        private void OnEnable()
        {
            EditorBuildSettings.sceneListChanged += SceneListChanged;
        }

        private void OnDisable()
        {
            EditorBuildSettings.sceneListChanged -= SceneListChanged;
        }

        private void SceneListChanged()
        {
            UpdateSceneList();

            if (_autoListUpdate)
            {
                GenerateSceneEnum();
            }
        }

        /// <summary>
        /// ビルドセッティングでシーンリストが変更されたら更新する
        /// </summary>
        public void UpdateSceneList()
        {
            _sceneList = EditorBuildSettings.scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();
        }

        /// <summary>
        /// シーン名のEnumを生成する
        /// </summary>
        public void GenerateSceneEnum()
        {
            EnumGenerator.EnumGenerate(SceneList, nameof(SceneList));
        }
    }

    /// <summary>
    /// Enum生成用のボタンを実行
    /// </summary>
    [CustomEditor(typeof(SceneManagerConfig))]
    public class MyScriptEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            var manager = target as SceneManagerConfig;

            if (GUILayout.Button("SceneListを読み込む"))
            {
                manager.UpdateSceneList();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Enumを生成する"))
            {
                manager.GenerateSceneEnum();
            }
        }
    }
}
