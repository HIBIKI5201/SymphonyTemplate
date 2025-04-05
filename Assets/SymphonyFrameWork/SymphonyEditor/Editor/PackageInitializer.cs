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
            string path = $"Packages/{SymphonyConstant.SYMPHONY_PACKAGE}/Enum"; //パッケージ内のEnumフォルダ

            if (Directory.Exists(path))
            {
                //パッケージ内のEnumを消す
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                AssetDatabase.Refresh();

                if (!Directory.Exists(EditorSymphonyConstant.ENUM_PATH))
                {
                    //Enumファイルを生成する
                    AutoEnumGenerator.SceneListEnumGenerate();
                    AutoEnumGenerator.TagsEnumGenerate();
                    AutoEnumGenerator.LayersEnumGenerate();
                    AutoEnumGenerator.AudioEnumGenerate();
                }
                else
                {
                    var enumAsmdefPath = Path.Combine(EditorSymphonyConstant.ENUM_PATH, "SymphonyFrameWork.Enum.asmdef");
                    var mainAsmdefPath = EditorSymphonyConstant.FRAMEWORK_PATH() + "/SymphonyFrameWork.asmdef";
                    AssemblyGenerator.AddAsssemblyReference(mainAsmdefPath, enumAsmdefPath);
                }
            }
        }
    }
}
