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
                AutoEnumGenerator.SceneListEnumGenerate();
                AutoEnumGenerator.TagsEnumGenerate();
                AutoEnumGenerator.LayersEnumGenerate();
                AutoEnumGenerator.AudioEnumGenerate();
            }
        }
    }
}
