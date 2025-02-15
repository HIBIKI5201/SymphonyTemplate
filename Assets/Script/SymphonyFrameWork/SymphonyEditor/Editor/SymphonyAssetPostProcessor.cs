using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkのディレクトリを保護するクラス
    /// </summary>
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

                // 移動がSymphonyFrameWorkのアセットかどうかを判定
                if (oldPath.StartsWith(protectedPath))
                {
                    EditorUtility.DisplayDialog(
                    "移動禁止",
                    $"SymphonyFrameWorkは移動できません。\npath : '{oldPath}'",
                    "OK");

                    // 移動を元に戻す
                    AssetDatabase.MoveAsset(newPath, oldPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
