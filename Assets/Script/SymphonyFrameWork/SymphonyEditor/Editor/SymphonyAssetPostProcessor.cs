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

                // �ړ����ی�t�H���_���̃A�Z�b�g���ǂ����𔻒�
                if (oldPath.StartsWith(protectedPath))
                {
                    EditorUtility.DisplayDialog(
                    "�ړ��֎~",
                    $"'{oldPath}' �͈ړ��ł��܂���B",
                    "OK");

                    // �ړ������ɖ߂�
                    AssetDatabase.MoveAsset(newPath, oldPath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
