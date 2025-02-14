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
        /// �p�b�P�[�W�����[�h����Ă��邩�`�F�b�N����
        /// </summary>
        [MenuItem("Window/Symphony FrameWork/" + nameof(SymphonyPackageLoader))]
        private static void MenuExecution() => CheckAndInstallPackagesAsync(false);

        private static async void CheckAndInstallPackagesAsync(bool isEnterEditor)
        {
            //�p�b�P�[�W�}�l�[�W���[�̏��������I����Ă��邩
            if (Client.List() == null)
            {
                return;
            }

            // �p�b�P�[�W���X�g��񓯊��Ŏ擾
            var installedPackages = await GetInstalledPackagesAsync();

            if (installedPackages == null)
            {
                return;
            }

            var missingPackages = requirePackages
                .Where(pkg => !installedPackages
                .Any(installedPkg => installedPkg.name == "com.unity." + pkg)).ToArray();

            //�p�b�P�[�W���Ȃ��ꍇ�͏I��
            if (missingPackages.Length <= 0)
            {
                if (!isEnterEditor && EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                "�S�Ẵp�b�P�[�W���C���X�g�[������Ă��܂�",
                "OK"))
                {
                    return;
                }
            }

            if (EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}", 
                "�ȉ��̃p�b�P�[�W���C���X�g�[�����܂�\n" + string.Join('\n', missingPackages),
                "OK","Cancel"))
            {
                await InstallPackageAsync(missingPackages); 
            }
        }

        /// <summary>
        /// �C���X�g�[������Ă���p�b�P�[�W��Ԃ�
        /// </summary>
        /// <returns></returns>
        private static async Task<PackageCollection> GetInstalledPackagesAsync()
        {
            ListRequest listRequest = Client.List();

            // IAsyncOperation ��񓯊��^�X�N�őҋ@
            await SymphonyTask.WaitUntil(() => listRequest.IsCompleted);

            if (listRequest.Status == StatusCode.Failure)
            {
                Debug.LogError("Failed to fetch package list: " + listRequest.Error.message);
                return null;
            }

            return listRequest.Result;
        }

        /// <summary>
        /// �p�b�P�[�W�����[�h����
        /// </summary>
        /// <param name="packageNames"></param>
        /// <returns></returns>
        private static async Task InstallPackageAsync(string[] packageNames)
        {
            foreach (var name in packageNames)
            {
                AddRequest addRequest = Client.Add("com.unity." + name);

                // IAsyncOperation ��񓯊��^�X�N�őҋ@
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