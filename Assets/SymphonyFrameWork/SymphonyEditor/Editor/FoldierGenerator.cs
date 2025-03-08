using SymphonyFrameWork.Core;
using SymphonyFrameWork.Debugger;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class FolderGenerator
    {
        [MenuItem(SymphonyConstant.MENU_PATH + nameof(FolderGenerator), priority = 100)]
        public static void GenerateFolder()
        {
            string assetsPath = "Assets";
            string artPath = "Arts";
            string animationPath =  "Animation";

            string[] assetsFolders =
                // アセット直下のフォルダ
                new string[] { artPath, "AssetStoreTools", "Editor", "Resources", "Prefabs", "Scenes", "Scripts", "Settings" }
                //Artフォルダ内のフォルダ
                .Concat(new string[] { animationPath, "Audio", "Materials", "Meshes", "Textures", "Shaders", "Sprites" }
                    .Select(s => $"{artPath}/{s}"))
                //Animationのフォルダ
                .Concat(new string[] { "Clips", "Controllers" }
                    .Select(s => $"{artPath}/{animationPath}/{s}"))
                .ToArray();
            

            foreach (string folder in assetsFolders)
            {
                string path = $"{assetsPath}/{folder}";

                if (!AssetDatabase.IsValidFolder(path))
                {
                    FolderCreate(path);
                    SymphonyDebugLog.AddText($"フォルダ作成: {path}");
                }
            }
            AssetDatabase.Refresh();

            SymphonyDebugLog.TextLog();
            EditorUtility.DisplayDialog("フォルダを生成", "フォルダを生成しました", "OK");
        }

        private static void FolderCreate(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            string parent = Path.GetDirectoryName(path);
            string folderName = Path.GetFileName(path);

            if (!AssetDatabase.IsValidFolder(parent))
            {
                FolderCreate(parent);
            }

            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
