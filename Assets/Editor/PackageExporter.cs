using UnityEditor;
using UnityEngine;

public class PackageExporter : EditorWindow
{
    [MenuItem("Tools/Export SymhpnyFrameWork Package")]
    public static void ExportPackage()
    {
        // �G�N�X�|�[�g�������t�H���_�̃p�X
        string folderPath = "Assets/Script/SymphonyFrameWork";

        // �ۑ���̃p�X�i�G�N�X�|�[�g��j
        string exportPath = @"C:\Users\takut\OneDrive\�f�X�N�g�b�v\Sinfonia Studio\SymphonyFrameWork.unitypackage";

        // UnityPackage���쐬
        string[] files = new string[] { folderPath };
        AssetDatabase.ExportPackage(files, exportPath, 
            ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);

        Debug.Log("Package exported to: " + exportPath);
    }
}
