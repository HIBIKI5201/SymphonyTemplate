using UnityEditor;
using UnityEngine;

public class PackageExporter : EditorWindow
{
    [MenuItem("Tools/Export SymhpnyFrameWork Package")]
    public static void ExportPackage()
    {
        // エクスポートしたいフォルダのパス
        var folderPath = "Assets/Script/SymphonyFrameWork";

        // 保存先のパス（エクスポート先）
        var exportPath = @"C:\Users\takut\OneDrive\デスクトップ\Sinfonia Studio\SymphonyFrameWork.unitypackage";

        // UnityPackageを作成
        var files = new[] { folderPath };
        AssetDatabase.ExportPackage(files, exportPath,
            ExportPackageOptions.Recurse);

        Debug.Log("Package exported to: " + exportPath);
    }
}