using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkのディレクトリを保護するクラス
    /// </summary>
    public class SymphonyAssetPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            SymphonyFileDontMove(movedAssets, movedFromAssetPaths);
        }

        /// <summary>
        /// SymphonyFrameWorkフォルダ内の物が移動されたら戻す
        /// </summary>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        private static void SymphonyFileDontMove(string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                string oldPath = movedFromAssetPaths[i];
                string newPath = movedAssets[i];

                // 移動がSymphonyFrameWorkのアセットかどうかを判定
                if (oldPath.StartsWith(SymphonyConstant.FRAMEWORK_PATH))
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
