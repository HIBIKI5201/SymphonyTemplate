using SymphonyFrameWork.Utility;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// 有用なパッケージを自動インストールするクラス
    /// </summary>
    [InitializeOnLoad]
    public static class SymphonyPackageLoader
    {
        private static readonly string[] requirePackages = new string[]
        {
            "ai.navigation",
            "addressables",
            "cinemachine",
            "behavior",
            "formats.fbx",
            "probuilder",
            "postprocessing",
            "memoryprofiler",
            "visualeffectgraph",
        };

        //static SymphonyPackageLoader() => EditorApplication.delayCall += () => CheckAndInstallPackagesAsync(true);
        //試した事
        //InitializeOnLoad => コンパイル時（Play時など）に実行される上に
        //                    実行タイミングが早すぎてエディタの初期化前に実行される
        //EditorApplication.delayCall => Play時に実行される
        //SessionState => 実行タイミングが早い問題のせいで処理に失敗して終わる
        //EditorPrefs => 上記に加え再起動しても実行されない
        //EditorApplication.update => 非同期実行している間にタスクが重複していく



        /// <summary>
        /// パッケージがロードされているかチェックする
        /// </summary>
        [MenuItem(SymphonyConstant.MENU_PATH + nameof(SymphonyPackageLoader), priority = 100)]
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

            var missingPackages = GetMissingPackages(requirePackages, installedPackages);

            //パッケージがない場合は終了
            if (missingPackages.Length < 1)
            {
                if (!isEnterEditor && EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                "全てのパッケージがインストールされています",
                "OK"))
                {
                    return;
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                    "以下のパッケージをインストールします\n" + string.Join('\n', missingPackages),
                    "OK", "Cancel"))
                {
                    await InstallPackageAsync(missingPackages);
                }
            }
        }

        /// <summary>
        /// インストールされているパッケージを返す
        /// </summary>
        /// <returns></returns>
        private static async Task<PackageCollection> GetInstalledPackagesAsync()
        {
            EditorUtility.DisplayProgressBar(nameof(SymphonyPackageLoader), "パッケージを確認中", 0);

            ListRequest listRequest = Client.List();

            float timer = Time.time;
            // IAsyncOperation を非同期タスクで待機
            await SymphonyTask.WaitUntil(() => listRequest.IsCompleted || timer + 60 < Time.time);

            EditorUtility.ClearProgressBar();

            if (timer + 60 < Time.time)
            {
                EditorUtility.DisplayDialog(nameof(SymphonyPackageLoader), "タイムアウトしました", "OK");
            }

            if (listRequest.Status == StatusCode.Failure)
            {
                Debug.LogError("Failed to fetch package list: " + listRequest.Error.message);
                return null;
            }

            return listRequest.Result;
        }

        /// <summary>
        /// パッケージがロードされているかチェックする
        /// </summary>
        private static string[] GetMissingPackages(string[] required, PackageCollection installedPackages)
        {
            var missingPackages = new ConcurrentBag<string>();

            Parallel.ForEach(required, pkg =>
            {
                string fullPackageName = "com.unity." + pkg;

                if (!installedPackages.Any(installedPkg => installedPkg.name == fullPackageName))
                {
                    missingPackages.Add(fullPackageName);
                }
            });

            return missingPackages.ToArray();
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
            }
        }


    }
}