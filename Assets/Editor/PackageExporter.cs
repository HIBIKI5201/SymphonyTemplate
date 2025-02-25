﻿using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SymphonyFrameWork.Core;
using SymphonyFrameWork.Editor;

public class PackageExporter : EditorWindow
{
    [MenuItem("Tools/Export SymphonyFrameWork Package")]
    public static void ExportPackage()
    {
        //Enumを初期化
        EnumGenerator.EnumGenerate(Array.Empty<string>(), "SceneList");
        
        // エクスポート対象のフォルダ
        var folderPath = "Assets/Script/SymphonyFrameWork";

        // 除外するリソース系のフォルダ
        var excludePaths = new string[]
        {
            SymphonyConstant.RESOURCES_RUNTIME_PATH,
            SymphonyConstant.RESOURCES_EDITOR_PATH,
        };

        var parentPaths = new string[]
        {
            //親フォルダ
            SymphonyConstant.FRAMEWORK_PATH,
            SymphonyConstant.FRAMEWORK_PATH + "/Runtime",
            SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor",
            SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor/Editor",
        };

        // フォルダ内のすべてのアセットを取得
        string[] assetGUIDs = AssetDatabase.FindAssets(string.Empty, new[] { folderPath });
        List<string> assetPaths = assetGUIDs
            .Select(AssetDatabase.GUIDToAssetPath)
            //親フォルダを除外
            .Where(path => parentPaths.All(parent => path != parent))
            //リソース系フォルダに含まれているファイルを除外
            .Where(path => !excludePaths.Any(exclude => path.StartsWith(exclude)))
            .ToList();

        if (assetPaths.Count == 0)
        {
            Debug.LogWarning("エクスポート対象のアセットがありません。");
            return;
        }
        
        Debug.Log(string.Join("\n", assetPaths));

        // 保存先のパス（エクスポート先）
        var exportPath = @"C:\Users\takut\OneDrive\デスクトップ\Sinfonia Studio\SymphonyFrameWork.unitypackage";

        // UnityPackageを作成
        AssetDatabase.ExportPackage(assetPaths.ToArray(), exportPath, ExportPackageOptions.Recurse);

        Debug.Log("Package exported to: " + exportPath);
    }
}
