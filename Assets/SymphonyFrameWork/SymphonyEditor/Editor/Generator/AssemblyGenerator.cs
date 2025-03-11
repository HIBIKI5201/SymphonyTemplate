using SymphonyFrameWork.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    ///     アセンブリを自動生成する
    /// </summary>
    public static class AssemblyGenerator
    {
        public static void CreateEnumAssembly()
        {
            string enumAsmdefPath = Path.Combine(EditorSymphonyConstant.ENUM_PATH, "SymphonyFrameWork.Enum.asmdef");
            string mainAsmdefPath = EditorSymphonyConstant.FRAMEWORK_PATH() + "/SymphonyFrameWork.asmdef";

            // SymphonyFrameWork.Enum.asmdef の生成
            if (!File.Exists(enumAsmdefPath))
            {
                string directoryPath = Path.GetDirectoryName(enumAsmdefPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var enumAsmdef = new AssemblyDefinitionData("SymphonyFrameWork.Enum");

                // アセンブリ定義ファイルを作成
                string json = JsonUtility.ToJson(enumAsmdef, true);
                File.WriteAllText(enumAsmdefPath, json);
                AssetDatabase.ImportAsset(enumAsmdefPath); // アセットデータベースにインポート
            }

            // SymphonyFrameWork.asmdef に参照を追加
            if (File.Exists(mainAsmdefPath))
            {
                string mainAsmdefJson = File.ReadAllText(mainAsmdefPath);
                var mainAsmdef = JsonUtility.FromJson<AssemblyDefinitionData>(mainAsmdefJson);

                string enumAsmdefGUID = "GUID:" + AssetDatabase.AssetPathToGUID(enumAsmdefPath);

                // 参照がすでに追加されていない場合にのみ追加

                if (!mainAsmdef.references.Contains(enumAsmdefGUID))
                {
                    var referencesList = new List<string>(mainAsmdef.references)
                {
                    enumAsmdefGUID
                };
                    mainAsmdef.references = referencesList.ToArray();

                    // 変更を反映
                    string updatedJson = JsonUtility.ToJson(mainAsmdef, true);
                    File.WriteAllText(mainAsmdefPath, updatedJson);
                    AssetDatabase.ImportAsset(mainAsmdefPath); // アセットデータベースにインポート
                }
            }
            else
            {
                Debug.LogError("SymphonyFrameWork.asmdef が見つかりません。");
            }
        }

        [Serializable]
        private class AssemblyDefinitionData
        {
            public string name = string.Empty;
            public string rootNamespace = string.Empty;
            public string[] references = new string[0];
            public string[] includePlatforms = new string[0];
            public string[] excludePlatforms = new string[0];
            public bool allowUnsafeCode = false;
            public bool overrideReferences = false;
            public string[] precompiledReferences = new string[0];
            public bool autoReferenced = true;
            public string[] defineConstraints = new string[0];
            public string[] versionDefines = new string[0];
            public bool noEngineReferences = false;
            public string[] platforms = new string[0];

            public AssemblyDefinitionData(string name)
            {
                this.name = name;
            }
        }
    }
}
