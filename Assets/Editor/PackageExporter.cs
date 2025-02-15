using UnityEditor;
using UnityEngine;

public class PackageExporter : EditorWindow
{
    [MenuItem("Tools/Export SymhpnyFrameWork Package")]
    public static void ExportPackage()
    {
        // エクスポートしたいフォルダのパス
        string folderPath = "Assets/Script/SymphonyFrameWork";

        // 保存先のパス（エクスポート先）
        string exportPath = @"C:\Users\takut\OneDrive\デスクトップ\Sinfonia Studio\SymphonyFrameWork.unitypackage";

        // UnityPackageを作成
        string[] files = new string[] { folderPath };
        AssetDatabase.ExportPackage(files, exportPath, 
            ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies);

        Debug.Log("Package exported to: " + exportPath);
    }
}
