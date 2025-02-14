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
            "addressables",
            "cinemachine",
            "probuilder",
            "behavior",
            "postprocessing",
            "memoryprofiler",
            "visualeffectgraph"
        };

        private const string SESSION_KEY = nameof(SymphonyPackageLoader);

        static SymphonyPackageLoader()
        {
            //起動直後かどうか判定する
            if (!SessionState.GetBool(SESSION_KEY, false))
            {
                SessionState.SetBool(SESSION_KEY, true);
                CheckAndInstallPackagesAsync();
            }
        }

        /// <summary>
        /// パッケージがロードされているかチェックする
        /// </summary>
        private static async void CheckAndInstallPackagesAsync()
        {
            // パッケージリストを非同期で取得
            var installedPackages = await GetInstalledPackagesAsync();

            var missingPackages = requirePackages
                .Where(pkg => !installedPackages
                .Any(installedPkg => installedPkg.name == "com.unity." + pkg)).ToArray();

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
        private static async Task<PackageCollection> GetInstalledPackagesAsync()
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
        private static async Task InstallPackageAsync(string[] packageNames)
        {
            foreach (var name in packageNames)
            {
                AddRequest addRequest = Client.Add("com.unity." + name);

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
            }
        }
    }
}