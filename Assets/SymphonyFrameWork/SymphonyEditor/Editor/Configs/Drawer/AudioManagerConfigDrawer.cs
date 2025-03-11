using SymphonyFrameWork.Core;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    [CustomEditor(typeof(AudioManagerConfig))]
    public class AudioManagerConfigDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = target as AudioManagerConfig;
            if (GUILayout.Button($"{EditorSymphonyConstant.AudioGroupTypeEnumName}Enumを再生成"))
            {
                EnumGenerator.EnumGenerate(
                    myScript.AudioGroupSettingList.Select(s => s.AudioGroupName).ToArray(),
                    EditorSymphonyConstant.AudioGroupTypeEnumName);
            }
        }

    }
}
