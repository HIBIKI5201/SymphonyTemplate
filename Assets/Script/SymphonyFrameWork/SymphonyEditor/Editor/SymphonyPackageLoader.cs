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
        private static bool _isCheck = false;

        private static readonly string[] requirePackages = new string[]
        {
            "com.unity.addressables",
            "com.unity.cinemachine",
            "com.unity.probuilder",
            "com.unity.behavior"
        };

        static SymphonyPackageLoader()
        {
            if (!_isCheck)
            {
                _isCheck = true;
                CheckAndInstallPackagesAsync();
            }
        }

        /// <summary>
        /// パッケージがロードされているかチェックする
        /// </summary>
        static async void CheckAndInstallPackagesAsync()
        {
            // パッケージリストを非同期で取得
            var installedPackages = await GetInstalledPackagesAsync();

            var missingPackages = requirePackages.Where(pkg => !installedPackages.Any(installedPkg => installedPkg.name == pkg)).ToArray();

            //パッケージがない場合は終了
            if (missingPackages.Length <= 0)
            {
                return;
            }

            if (EditorUtility.DisplayDialog("SymphonyPackageLoader",
                $"{string.Join('\n', missingPackages)} をインストールしします",
                "OK"))
            {
                await InstallPackageAsync(missingPackages);
            }
        }

        /// <summary>
        /// インストールされているパッケージを返す
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// パッケージをロードする
        /// </summary>
        /// <param name="packageNames"></param>
        /// <returns></returns>
        static async Task InstallPackageAsync(string[] packageNames)
        {
            EditorUtility.DisplayProgressBar("パッケージのインストール",
                "必要なパッケージをインストールしています",
                0f);

            int count = 0;
            foreach (var name in packageNames)
            {
                AddRequest addRequest = Client.Add(name);

                // IAsyncOperation を非同期タスクで待機
                await SymphonyTask.WaitUntil(() => addRequest.IsCompleted);

                if (addRequest.Status == StatusCode.Failure)
                {
                    Debug.LogError("Failed to install package: " + addRequest.Error.message);
                }
                else
                {
                    Debug.Log("Package installed: " + name);
                }

                count++;

                EditorUtility.DisplayProgressBar("パッケージのインストール",
                    "必要なパッケージをインストールしています",
                    count / packageNames.Length);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}