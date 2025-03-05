using SymphonyFrameWork.Core;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    [InitializeOnLoad]
    public static class AutoEnumGenerator
    {
        private static readonly AutoEnumGeneratorConfig _config;

        static AutoEnumGenerator()
        {
            _config = SymphonyEditorConfigLocator.GetConfig<AutoEnumGeneratorConfig>();

            TagsAndLayersPostProcessor.SceneList.OnSettingChanged -= SceneListChanged;
            TagsAndLayersPostProcessor.SceneList.OnSettingChanged += SceneListChanged;

            TagsAndLayersPostProcessor.Tags.OnSettingChanged -= TagsChanged;
            TagsAndLayersPostProcessor.Tags.OnSettingChanged += TagsChanged;

            TagsAndLayersPostProcessor.Layers.OnSettingChanged -= LayersChanged;
            TagsAndLayersPostProcessor.Layers.OnSettingChanged += LayersChanged;
        }

        /// <summary>
        ///     シーンリスト変更時の更新
        /// </summary>
        private static void SceneListChanged()
        {
            if (_config.AutoSceneListUpdate)
            {
                //シーンリストのシーン名を取得
                var sceneList = EditorBuildSettings.scenes
                    .Select(s => Path.GetFileNameWithoutExtension(s.path))
                    .ToArray();

                //シーン名のEnumを生成する
                EnumGenerator.EnumGenerate(sceneList,
                    SymphonyConstant.EditorSymphonyConstrant.SceneListEnumFileName);
            }
        }

        private static void TagsChanged()
        {
            string[] tags = InternalEditorUtility.tags;
            EnumGenerator.EnumGenerate(tags,
                SymphonyConstant.EditorSymphonyConstrant.TagsEnumFileName, true);
        }

        private static void LayersChanged()
        {
            string[] layers = InternalEditorUtility.layers;
            EnumGenerator.EnumGenerate(layers,
                SymphonyConstant.EditorSymphonyConstrant.LayersEnumFileName, true);
        }
    }
}