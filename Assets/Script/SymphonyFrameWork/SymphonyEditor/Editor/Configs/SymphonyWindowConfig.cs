using System.IO;
using System.Linq;
using SymphonyFrameWork.Attribute;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public class SymphonyWindowConfig : ScriptableObject
    {
        [ReadOnly, SerializeField]
        private bool _autoSceneListUpdate = true;
        public bool AutoSceneListUpdate { get => _autoSceneListUpdate; set=> _autoSceneListUpdate = value; }
        
        private void OnEnable()
        {
            EditorBuildSettings.sceneListChanged -= SceneListChanged;
            EditorBuildSettings.sceneListChanged += SceneListChanged;
        }
        
        /// <summary>
        /// シーンリスト変更時の更新
        /// </summary>
        private void SceneListChanged()
        {
            if (_autoSceneListUpdate)
            {
                var sceneList = UpdateSceneList();
                GenerateSceneEnum(sceneList);
            }
        }

        /// <summary>
        /// ビルドセッティングでシーンリストが変更されたら更新する
        /// </summary>
        public string[] UpdateSceneList()
        {
            return EditorBuildSettings.scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();
        }

        /// <summary>
        /// シーン名のEnumを生成する
        /// </summary>
        public void GenerateSceneEnum(string[] sceneList)
        {
            EnumGenerator.EnumGenerate(sceneList, "SceneList");
        }
    }
}
