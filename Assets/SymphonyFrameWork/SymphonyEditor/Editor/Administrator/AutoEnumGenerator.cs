using SymphonyFrameWork.Core;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace SymphonyFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AutoEnumGenerator
    {
        private static readonly AutoEnumGeneratorConfig _config;

        static AutoEnumGenerator()
        {
            _config = SymphonyEditorConfigLocator.GetConfig<AutoEnumGeneratorConfig>();

            TagsAndLayersPostProcessor.SceneList.OnSettingChanged -= SceneListEnumGenerate;
            TagsAndLayersPostProcessor.SceneList.OnSettingChanged += SceneListEnumGenerate;

            TagsAndLayersPostProcessor.Tags.OnSettingChanged -= TagsEnumGenerate;
            TagsAndLayersPostProcessor.Tags.OnSettingChanged += TagsEnumGenerate;

            TagsAndLayersPostProcessor.Layers.OnSettingChanged -= LayersEnumGenerate;
            TagsAndLayersPostProcessor.Layers.OnSettingChanged += LayersEnumGenerate;

            if (!FileCheck(EditorSymphonyConstrant.SceneListEnumFileName))
                SceneListEnumGenerate();

            if (!FileCheck(EditorSymphonyConstrant.TagsEnumFileName))
                TagsEnumGenerate();

            if (!FileCheck(EditorSymphonyConstrant.LayersEnumFileName))
                LayersEnumGenerate();
        }

        public static bool FileCheck(string fileName)
        {
            string path = EnumGenerator.GetEnumFilePath(fileName);
            return File.Exists(path);
        }

        /// <summary>
        ///     シーンリスト変更時の更新
        /// </summary>
        private static void SceneListEnumGenerate()
        {
            if (_config.AutoSceneListUpdate)
            {
                //シーンリストのシーン名を取得
                var sceneList = EditorBuildSettings.scenes
                    .Select(s => Path.GetFileNameWithoutExtension(s.path))
                    .ToArray();

                //シーン名のEnumを生成する
                EnumGenerator.EnumGenerate(sceneList,
                    EditorSymphonyConstrant.SceneListEnumFileName);
            }
        }

        private static void TagsEnumGenerate()
        {
            if (_config.AutoTagsUpdate)
            {
                EnumGenerator.EnumGenerate(InternalEditorUtility.tags,
                    EditorSymphonyConstrant.TagsEnumFileName, true);
            }
        }

        private static void LayersEnumGenerate()
        {
            if (_config.AutoLayerUpdate)
            {
                EnumGenerator.EnumGenerate(InternalEditorUtility.layers,
                    EditorSymphonyConstrant.LayersEnumFileName, true);
            }
        }
    }
}