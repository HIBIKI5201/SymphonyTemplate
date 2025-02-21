using System;
using System.IO;
using System.Linq;
using SymphonyFrameWork.Config;
using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AutoEnumGenerator
    {
        private static readonly AutoEnumGeneratorConfig Config;

        private const string SceneListFileName = "SceneList";
        static AutoEnumGenerator()
        {
            SceneListCheck();
            
            Config = SymphonyConfigLocator.GetConfig<AutoEnumGeneratorConfig>();

            EditorBuildSettings.sceneListChanged -= SceneListChanged;
            EditorBuildSettings.sceneListChanged += SceneListChanged;
        }

        /// <summary>
        ///     シーンリスト変更時の更新
        /// </summary>
        private static void SceneListChanged()
        {
            if (Config.AutoSceneListUpdate)
            {
                //シーンリストのシーン名を取得
                var sceneList = EditorBuildSettings.scenes
                    .Select(s => Path.GetFileNameWithoutExtension(s.path))
                    .ToArray();

                //シーン名のEnumを生成する
                EnumGenerator.EnumGenerate(sceneList, SceneListFileName);
            }
        }

        /// <summary>
        /// シーンリストがない場合を生成する
        /// </summary>
        private static void SceneListCheck()
        {
            string path = EnumGenerator.GetEnumFilePath(SceneListFileName);
            
            if (AssetDatabase.AssetPathExists(path)) return;
            
            EnumGenerator.EnumGenerate(Array.Empty<string>(), SceneListFileName);
        }
    }
}