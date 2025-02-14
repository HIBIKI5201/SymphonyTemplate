using UnityEditor;

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

                // �ړ���SymphonyFrameWork�̃A�Z�b�g���ǂ����𔻒�
                if (oldPath.StartsWith(protectedPath))
                {
                    EditorUtility.DisplayDialog(
                    "�ړ��֎~",
                    $"SymphonyFrameWork�͈ړ��ł��܂���B\npath : '{oldPath}'",
                    "OK");

                    // �ړ������ɖ߂�
                    AssetDatabase.MoveAsset(newPath, oldPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
