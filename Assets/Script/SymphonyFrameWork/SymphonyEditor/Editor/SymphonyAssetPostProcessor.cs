using UnityEditor;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkのディレクトリを保護するクラス
    /// </summary>
    public class SymphonyAssetPostProcessor : AssetPostprocessor
    {
        private const string LOCK_PATH = SymphonyConstant.MENU_PATH + "Symphony Asset Lock";

        static SymphonyAssetPostProcessor()
        {
            // Unityエディタが再起動された後でも状態が反映されるようにする
            EditorApplication.delayCall += ToggleOption;
        }

        /// <summary>
        /// 押されたらチェックを反転する
        /// </summary>
        [MenuItem(LOCK_PATH, priority = 200)]
        static void ToggleOption()
        {
            // 現在のチェック状態を取得
            bool isChecked = EditorPrefs.GetBool(LOCK_PATH, true);

            isChecked = !isChecked;

            // チェック状態を更新
            EditorPrefs.SetBool(LOCK_PATH, isChecked);
            Menu.SetChecked(LOCK_PATH, isChecked);
        }

        /// <summary>
        /// メニューのチェック表示を最新に保つ
        /// </summary>
        /// <returns></returns>
        [MenuItem(LOCK_PATH, true)]
        static bool ValidateLock()
        {
            bool isChecked = EditorPrefs.GetBool(LOCK_PATH, true);
            Menu.SetChecked(LOCK_PATH, isChecked);

            return true;
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
                    bool isLock = EditorPrefs.GetBool(LOCK_PATH, true);

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

