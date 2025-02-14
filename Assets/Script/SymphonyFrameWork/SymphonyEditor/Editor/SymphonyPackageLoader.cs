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

        static SymphonyPackageLoader() => EditorApplication.delayCall += () => CheckAndInstallPackagesAsync(true);

        /// <summary>
        /// パッケージがロードされているかチェックする
        /// </summary>
        [MenuItem("Window/Symphony FrameWork/" + nameof(SymphonyPackageLoader))]
        private static void MenuExecution() => CheckAndInstallPackagesAsync(false);

        private static async void CheckAndInstallPackagesAsync(bool isEnterEditor)
        {
            //パッケージマネージャーの初期化が終わっているか
            if (Client.List() == null)
            {
                return;
            }

            // パッケージリストを非同期で取得
            var installedPackages = await GetInstalledPackagesAsync();

            if (installedPackages == null)
            {
                return;
            }

            var missingPackages = requirePackages
                .Where(pkg => !installedPackages
                .Any(installedPkg => installedPkg.name == "com.unity." + pkg)).ToArray();

            //パッケージがない場合は終了
            if (missingPackages.Length <= 0)
            {
                if (!isEnterEditor && EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                "全てのパッケージがインストールされています",
                "OK"))
                {
                    return;
                }
            }

            if (EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}", 
                "以下のパッケージをインストールします\n" + string.Join('\n', missingPackages),
                "OK","Cancel"))
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