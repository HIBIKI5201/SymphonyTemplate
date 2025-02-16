using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkのディレクトリを保護するクラス
    /// </summary>
    public class SymphonyAssetPostProcessor : AssetPostprocessor
    {
        private const string LOCK_PATH = SymphonyConstant.MENU_PATH + nameof(SymphonyAssetPostProcessor);

        [MenuItem(LOCK_PATH, priority = 200)]
        static void ToggleOption()
        {
            // メニューのチェック状態を更新
            bool isChecked = Menu.GetChecked(LOCK_PATH);
            Menu.SetChecked(LOCK_PATH, !isChecked);
        }

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
                    bool isLock = Menu.GetChecked(LOCK_PATH);

                    //ロックされている時は移動できない
                    if (isLock)
                    {
                        if (EditorUtility.DisplayDialog(
                            "移動禁止",
                            $"SymphonyFrameWorkは移動できません\npath : '{oldPath}'",
                            "OK"))
                        {
                            // 移動を元に戻す
                            AssetDatabase.MoveAsset(newPath, oldPath);
                            AssetDatabase.Refresh();
                        }
                    }
                    //ロックされていない時は警告を出す
                    else
                    {
                        if (!EditorUtility.DisplayDialog(
                            "移動注意",
                            $"SymphonyFrameWorkを移動しようとしています。\n本当に移動しますか？\npath : '{oldPath}'",
                            "OK", "Cancel"))
                        {
                            // 移動を元に戻す
                            AssetDatabase.MoveAsset(newPath, oldPath);
                            AssetDatabase.Refresh();
                        }
                    }
                }
            }
        }
    }
}

