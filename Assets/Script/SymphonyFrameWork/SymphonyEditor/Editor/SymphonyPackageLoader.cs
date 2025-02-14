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
        /// �p�b�P�[�W�����[�h����Ă��邩�`�F�b�N����
        /// </summary>
        static async void CheckAndInstallPackagesAsync()
        {
            // �p�b�P�[�W���X�g��񓯊��Ŏ擾
            var installedPackages = await GetInstalledPackagesAsync();

            var missingPackages = requirePackages.Where(pkg => !installedPackages.Any(installedPkg => installedPkg.name == pkg)).ToArray();

            //�p�b�P�[�W���Ȃ��ꍇ�͏I��
            if (missingPackages.Length <= 0)
            {
                return;
            }

            if (EditorUtility.DisplayDialog("SymphonyPackageLoader",
                $"{string.Join('\n', missingPackages)} ���C���X�g�[�������܂�",
                "OK"))
            {
                await InstallPackageAsync(missingPackages);
            }
        }

        /// <summary>
        /// �C���X�g�[������Ă���p�b�P�[�W��Ԃ�
        /// </summary>
        /// <returns></returns>
        static async Task<PackageCollection> GetInstalledPackagesAsync()
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
        static async Task InstallPackageAsync(string[] packageNames)
        {
            EditorUtility.DisplayProgressBar("�p�b�P�[�W�̃C���X�g�[��",
                "�K�v�ȃp�b�P�[�W���C���X�g�[�����Ă��܂�",
                0f);

            int count = 0;
            foreach (var name in packageNames)
            {
                AddRequest addRequest = Client.Add(name);

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

                count++;

                EditorUtility.DisplayProgressBar("�p�b�P�[�W�̃C���X�g�[��",
                    "�K�v�ȃp�b�P�[�W���C���X�g�[�����Ă��܂�",
                    count / packageNames.Length);
            }

            EditorUtility.ClearProgressBar();
        }
    }
}