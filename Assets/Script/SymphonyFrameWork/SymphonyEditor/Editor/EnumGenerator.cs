using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class EnumGenerator
    {
        private const string FrameWork_Path = "Assets/Script/SymphonyFrameWork/";

        public static async void Method(string[] strings, string fileName)
        {
            HashSet<string> hash = new HashSet<string>(strings);

            var enumFilePath = $"{FrameWork_Path}{fileName}Enum.cs";
            if (File.Exists(enumFilePath))
            {
                File.Delete(enumFilePath);
            }

            IEnumerable<string> content = new[] { "public enum " + fileName + " : int {" };

            content = content.Concat(hash.Select(s => $"{s},"));
            content = content.Append("}");

            await File.WriteAllLinesAsync(enumFilePath, content, Encoding.UTF8);
        }
    }
}
