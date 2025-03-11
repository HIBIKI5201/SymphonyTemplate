using SymphonyFrameWork.Core;
using System;
using System.IO;
using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    ///     UPMからインストールされた時に初期化する
    /// </summary>
    [InitializeOnLoad]
    public static class PackageInitializer
    {
        static PackageInitializer()
        {
            string path = $"Packages/{SymphonyConstant.SYMPHONY_PACKAGE}/Enum";

            if (Directory.Exists(path))
            {
                //パッケージのEnumを消す
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                AssetDatabase.Refresh();

                //Enumファイルを生成する
                EnumGenerator.EnumGenerate(Array.Empty<string>(),
                    EditorSymphonyConstant.AudioGroupTypeEnumName);

                EnumGenerator.EnumGenerate(Array.Empty<string>(),
                    EditorSymphonyConstant.SceneListEnumFileName);

                EnumGenerator.EnumGenerate(Array.Empty<string>(),
                    EditorSymphonyConstant.TagsEnumFileName);

                EnumGenerator.EnumGenerate(Array.Empty<string>(),
                    EditorSymphonyConstant.LayersEnumFileName);
            }
        }
    }
}
