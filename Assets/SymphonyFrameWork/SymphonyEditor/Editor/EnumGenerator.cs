using SymphonyFrameWork.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class EnumGenerator
    {
        private static readonly Regex IdentifierRegex = new(@"^@?[a-zA-Z_][a-zA-Z0-9_]*$");
        private static readonly string[] ReservedWords = { "abstract", "as", "base", "bool", "break", "while" };

        public static async void EnumGenerate(string[] strings, string fileName, bool flag = false)
        {
            //重複を削除
            var hash = new HashSet<string>(new string[1] { "None" }.Concat(strings))
                .Where(s =>
                {
                    //文字列の頭文字がアルファベットではないものは除外
                    if (!IdentifierRegex.IsMatch(s))
                    {
                        Debug.LogWarning($"無効な文字で始まっているか無効な文字が含まれているため'{s}'を除外しました");
                        return false;
                    }

                    //プログラム文字を除外
                    if (ReservedWords.Contains(s)) Debug.LogWarning($"無効な文字列'{s}'を除外しました");

                    return true;
                })
                .ToHashSet();

            //ディレクトリを生成
            CreateResourcesFolder($"{SymphonyConstant.ENUM_PATH}/");

            //ファイル名を生成
            var enumFilePath = GetEnumFilePath(fileName);

            var content = !flag ? NormalEnumGenerate(fileName, hash) : FlagEnumGenerate(fileName, hash);

            await File.WriteAllLinesAsync(enumFilePath, content, Encoding.UTF8);
            File.SetLastAccessTime(enumFilePath, DateTime.Now);
            AssetDatabase.ImportAsset(enumFilePath, ImportAssetOptions.ForceUpdate);

            Debug.Log($"{fileName}Enumを生成しました");
        }

        public static string GetEnumFilePath(string fileName) => $"{SymphonyConstant.ENUM_PATH}/{fileName}Enum.cs";

        /// <summary>
        ///     リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder(string resourcesPath)
        {
            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.ImportAsset(resourcesPath, ImportAssetOptions.ForceUpdate);
            }

            CreateAssembly();

            AssetDatabase.Refresh();
        }

        private static void CreateAssembly()
        {
            string enumAsmdefPath = Path.Combine(SymphonyConstant.ENUM_PATH, "SymphonyFrameWork.Enum.asmdef");
            string mainAsmdefPath = "Assets/SymphonyFrameWork/SymphonyFrameWork.asmdef";

            // SymphonyFrameWork.Enum.asmdef の生成
            if (!File.Exists(enumAsmdefPath))
            {
                string directoryPath = Path.GetDirectoryName(enumAsmdefPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var enumAsmdef = new AssemblyDefinitionData
                {
                    name = "SymphonyFrameWork.Enum",
                    references = new string[0],  // 他のアセンブリは参照しない
                    includePlatforms = new string[0],
                    excludePlatforms = new string[0],
                    defineConstraints = new string[0],
                    allowUnsafeCode = false,
                    overrideReferences = false,
                    precompiledReferences = new string[0],
                    autoReferenced = true,
                    platforms = new string[0]
                };

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
        public class AssemblyDefinitionData
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public string[] defineConstraints;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] platforms;
        }

        /// <summary>
        ///     通常のEnumを生成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        private static IEnumerable<string> NormalEnumGenerate(string fileName, HashSet<string> hash)
        {
            //ファイルの中身を生成
            IEnumerable<string> content = new[] { "public enum " + fileName + "Enum : int\n{" };

            //Enumファイルに要素を追加していく
            content = content.Concat(hash.Select((s, i) => $"    {s} = {i},"));
            content = content.Append("}");

            return content;
        }

        private static IEnumerable<string> FlagEnumGenerate(string fileName, HashSet<string> hash)
        {
            //ファイルの中身を生成
            IEnumerable<string> content = new[]
                { "using System;\n\n[Flags]\npublic enum " + fileName + "Enum : int\n{" };

            //Enumファイルに要素を追加していく
            content = content.Concat(hash.Select((s, i) => $"    {s} = 1 << {i},"));
            content = content.Append("}");

            return content;
        }

        [MenuItem(SymphonyConstant.MENU_PATH + "Debug/" + nameof(CreateResourcesFolder), priority = 1000)]
        private static void CreateResourceFolderDebug() => CreateResourcesFolder($"{SymphonyConstant.ENUM_PATH}/");
    }
}