using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    public static class EnumGenerator
    {
        private const string FrameWork_Path = "Assets/Script/SymphonyFrameWork/Enum/";

        public static async void EnumGenerate(string[] strings, string fileName)
        {
            //重複を削除
            HashSet<string> hash = new HashSet<string>(strings);

            if (hash.Count <= 0)
            {
                return;
            }

            //ディレクトリを生成
            CreateResourcesFolder(FrameWork_Path);

            var enumFilePath = $"{FrameWork_Path}{fileName}Enum.cs";
            if (File.Exists(enumFilePath))
            {
                File.Delete(enumFilePath);
            }

            IEnumerable<string> content = new[] { $"public enum " + fileName + "Enum : int {" };

            content = content.Concat(hash.Select(s => $"{s},"));
            content = content.Append("}");

            await File.WriteAllLinesAsync(enumFilePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// リソースフォルダが無ければ生成
        /// </summary>
        private static void CreateResourcesFolder(string resourcesPath)
        {
            //リソースがなければ生成
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
