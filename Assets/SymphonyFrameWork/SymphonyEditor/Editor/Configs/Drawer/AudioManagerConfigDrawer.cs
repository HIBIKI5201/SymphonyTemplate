using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    [CustomEditor(typeof(AudioManagerConfig))]
    public class AudioManagerConfigDrawer : UnityEditor.Editor
    {
        private const string AudioGroupTypeEnumName = "AudioGroupType";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = target as AudioManagerConfig;
            if (GUILayout.Button($"{AudioGroupTypeEnumName}Enumを再生成"))
            {
                EnumGenerator.EnumGenerate(
                    myScript.AudioGroupSettingList.Select(s => s.AudioGroupName).ToArray(),
                    AudioGroupTypeEnumName);
            }
        }

    }
}
