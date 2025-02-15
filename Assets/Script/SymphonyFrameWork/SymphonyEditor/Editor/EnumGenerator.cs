using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class EnumGenerator
    {
        private const string FrameWork_Path = "\"Assets/Script/SymphonyFrameWork/";
        private static async void Method(string[] strings, string fileName)
        {
            var enumFilePath = $"{FrameWork_Path}{fileName}.cs";
            if (!File.Exists(enumFilePath))
            {
                File.Create(enumFilePath);
            }

            File.WriteAllText(enumFilePath, "", Encoding.UTF8);

            IEnumerable<string> content = new[] { "public enum scenarioVoiceEnum : int {" };

            content = content.Concat(strings);
            content = content.Append("}");

            await File.WriteAllLinesAsync(enumFilePath, content, Encoding.UTF8);
        }
    }
}
