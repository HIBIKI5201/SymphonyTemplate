using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public class SymphonyAssetPostProcessor : AssetPostprocessor
    {
        static string protectedPath = "Assets/Script/SymphonyFrameWork";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                string oldPath = movedFromAssetPaths[i];
                string newPath = movedAssets[i];

                // 移動が保護フォルダ内のアセットかどうかを判定
                if (oldPath.StartsWith(protectedPath) && !newPath.StartsWith(protectedPath))
                {
                    Debug.LogWarning($"移動禁止: {oldPath} を {newPath} に移動しようとしましたが、元の場所に戻します。");

                    // 移動を元に戻す
                    AssetDatabase.MoveAsset(newPath, oldPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
