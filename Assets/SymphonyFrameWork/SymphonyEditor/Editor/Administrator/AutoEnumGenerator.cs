using SymphonyFrameWork.Config;
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
        static AutoEnumGenerator()
        {
            var config = SymphonyEditorConfigLocator.GetConfig<AutoEnumGeneratorConfig>();

            TagsAndLayersPostProcessor.SceneList.Dispose();
            TagsAndLayersPostProcessor.SceneList.OnSettingChanged += () =>
                {
                    if (config.AutoSceneListUpdate) SceneListEnumGenerate();
                };

            TagsAndLayersPostProcessor.Tags.Dispose();
            TagsAndLayersPostProcessor.Tags.OnSettingChanged += () =>
            {
                if (config.AutoTagsUpdate) TagsEnumGenerate();
            };

            TagsAndLayersPostProcessor.Layers.Dispose();
            TagsAndLayersPostProcessor.Layers.OnSettingChanged += () =>
            {
                if (config.AutoLayerUpdate) LayersEnumGenerate();
            };
        }

        /// <summary>
        ///     シーンリスト変更時の更新
        /// </summary>
        public static void SceneListEnumGenerate()
        {
            //シーンリストのシーン名を取得
            var sceneList = EditorBuildSettings.scenes
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            //シーン名のEnumを生成する
            EnumGenerator.EnumGenerate(sceneList,
                EditorSymphonyConstant.SceneListEnumFileName);
        }

        public static void TagsEnumGenerate()
        {
            EnumGenerator.EnumGenerate(InternalEditorUtility.tags,
                EditorSymphonyConstant.TagsEnumFileName, true);
        }

        public static void LayersEnumGenerate()
        {
            EnumGenerator.EnumGenerate(InternalEditorUtility.layers,
                EditorSymphonyConstant.LayersEnumFileName, true);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void AudioEnumGenerate()
        {
            var config = SymphonyConfigLocator.GetConfig<AudioManagerConfig>();

            if (config)
            {
                EnumGenerator.EnumGenerate(
                        config.AudioGroupSettingList.Select(s => s.AudioGroupName).ToArray(),
                        EditorSymphonyConstant.AudioGroupTypeEnumName);
            }
            else
            {
                Debug.LogWarning("Audio group settings not found.");
            }
        }
    }
}