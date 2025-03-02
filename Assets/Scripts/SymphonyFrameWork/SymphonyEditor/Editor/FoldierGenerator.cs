using Codice.CM.SEIDInfo;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class FolderGenerator
    {
        [MenuItem("Tools/Create Default Folders")]
        public static void GenerateFolder()
        {
            string assetsPath = "Assets";
            string artPath = "Arts";

            // アセット直下のフォルダ
            string[] assetsFolders = { "AssetStoreTools", "Editor", "Scenes", "Scripts", "Prefabs", artPath };

            foreach (string folder in assetsFolders)
            {
                string path = $"{assetsPath}/{folder}";

                if (!AssetDatabase.IsValidFolder(path))
                {
                    FolderCreate(path);
                    Debug.Log($"フォルダ作成: {path}");
                }
            }

            // Arts フォルダ内のフォルダ
            string[] artFolders = new string[] { "Materials", "Textures", "Audio" }
            .Select(s => $"{artPath}/{s}")
                .ToArray();

            foreach (string folder in artFolders)
            {
                string path = GetPath(folder);

                if (!AssetDatabase.IsValidFolder(path))
                {
                    FolderCreate(path);
                    Debug.Log($"フォルダ作成: {path}");
                }
            }

            AssetDatabase.Refresh();

            string GetPath(string folder) => $"{assetsPath}/{folder}";
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
