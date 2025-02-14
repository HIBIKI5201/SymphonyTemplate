using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    [InitializeOnLoad]
    public static class SymphonyPackageLoader
    {
        private static readonly string[] requirePackages = new string[]
        {
            "com.unity.addressables",
        };

        static SymphonyPackageLoader()
        {
            ListRequest listRequest = Client.List();


            if (listRequest.IsCompleted)
            {
                if (listRequest.Status == StatusCode.Failure)
                {
                    Debug.LogError("Failed to fetch package list: " + listRequest.Error.message);
                    return;
                }

                // インストールされているパッケージの名前を取得
                var installedPackages = listRequest.Result;
                foreach (var packageName in requirePackages)
                {
                    var isPackageInstalled = installedPackages.Any(pkg => pkg.name == packageName);
                    if (!isPackageInstalled)
                    {
                        // パッケージがインストールされていない場合、インストールを試みる
                        //InstallPackage(packageName);

                        Debug.Log($"is package installed {isPackageInstalled}");
                    }
                }

                Debug.Log(string.Join(" ", installedPackages));
            };

            void InstallPackage(string name)
            {
                // パッケージをインストール
                AddRequest addRequest = Client.Add(name);

                // インストール処理後のコールバック
                EditorApplication.update += () =>
                {
                    if (addRequest.IsCompleted)
                    {
                        if (addRequest.Status == StatusCode.Failure)
                        {
                            Debug.LogError($"Failed to install package: {addRequest.Error.message}");
                        }
                        else
                        {
                            Debug.Log($"Package installed: {name}");
                        }

                        // 次回更新での処理を停止
                        EditorApplication.update -= () => { };
                    }
                };
            }
        }
    }
}
