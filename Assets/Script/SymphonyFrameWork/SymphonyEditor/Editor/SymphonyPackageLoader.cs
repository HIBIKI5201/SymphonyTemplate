using SymphonyFrameWork.Utility;
using System.Linq;
using System.Threading.Tasks;
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
            CheckAndInstallPackagesAsync();
        }


        static async void CheckAndInstallPackagesAsync()
        {
            // パッケージリストを非同期で取得
            var installedPackages = await GetInstalledPackagesAsync();

            foreach (var packageName in requirePackages)
            {
                var isPackageInstalled = installedPackages.Any(pkg => pkg.name == packageName);
                if (!isPackageInstalled)
                {
                    await InstallPackageAsync(packageName);
                }
            }
        }

        static async Task<PackageCollection> GetInstalledPackagesAsync()
        {
            ListRequest listRequest = Client.List();

            // IAsyncOperation を非同期タスクで待機
            await SymphonyTask.WaitUntil(() => listRequest.IsCompleted);

            if (listRequest.Status == StatusCode.Failure)
            {
                Debug.LogError("Failed to fetch package list: " + listRequest.Error.message);
                return null;
            }

            return listRequest.Result;
        }

        static async Task InstallPackageAsync(string packageName)
        {
            AddRequest addRequest = Client.Add(packageName);

            // IAsyncOperation を非同期タスクで待機
            await SymphonyTask.WaitUntil(() => addRequest.IsCompleted);

            if (addRequest.Status == StatusCode.Failure)
            {
                Debug.LogError("Failed to install package: " + addRequest.Error.message);
            }
            else
            {
                Debug.Log("Package installed: " + packageName);
            }
        }
    }
}