using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public static class FoldierGenerator
    {
        [MenuItem("Tools/Create Default Folders")]
        public static void GenerateFolder()
        {
            string assetsPath = "Assets/";

            string[] folders = { "Scenes", "Scripts", "Prefabs" };

            folders.Concat(new string[] { "Materials", "Textures", "Audio" }.Select(s => "Art/" + s));

            foreach (string folder in folders)
            {
                string path = Path.Combine(assetsPath, folder);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder(assetsPath.TrimEnd('/'), folder);
                    Debug.Log($"フォルダ作成: {path}");
                }
                else
                {
                    Debug.Log($"既に存在しています: {path}");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
