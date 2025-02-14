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
        //��������
        //InitializeOnLoad => �R���p�C�����iPlay���Ȃǁj�Ɏ��s�������
        //                    ���s�^�C�~���O���������ăG�f�B�^�̏������O�Ɏ��s�����
        //EditorApplication.delayCall => Play���Ɏ��s�����
        //SessionState => ���s�^�C�~���O���������̂����ŏ����Ɏ��s���ďI���
        //EditorPrefs => ��L�ɉ����ċN�����Ă����s����Ȃ�
        //EditorApplication.update => �񓯊����s���Ă���ԂɃ^�X�N���d�����Ă���



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

            var missingPackages = GetMissingPackages(requirePackages, installedPackages);

            //�p�b�P�[�W���Ȃ��ꍇ�͏I��
            if (missingPackages.Length < 1)
            {
                if (!isEnterEditor && EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                "�S�Ẵp�b�P�[�W���C���X�g�[������Ă��܂�",
                "OK"))
                {
                    return;
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog($"{nameof(SymphonyPackageLoader)}",
                    "�ȉ��̃p�b�P�[�W���C���X�g�[�����܂�\n" + string.Join('\n', missingPackages),
                    "OK", "Cancel"))
                {
                    await InstallPackageAsync(missingPackages);
                }
            }
        }

        /// <summary>
        /// �C���X�g�[������Ă���p�b�P�[�W��Ԃ�
        /// </summary>
        /// <returns></returns>
        private static async Task<PackageCollection> GetInstalledPackagesAsync()
        {
            EditorUtility.DisplayProgressBar(nameof(SymphonyPackageLoader), "�p�b�P�[�W���m�F��", 0);

            ListRequest listRequest = Client.List();

            float timer = Time.time;
            // IAsyncOperation ��񓯊��^�X�N�őҋ@
            await SymphonyTask.WaitUntil(() => listRequest.IsCompleted || timer + 60 < Time.time);

            EditorUtility.ClearProgressBar();

            if (timer + 60 < Time.time)
            {
                EditorUtility.DisplayDialog(nameof(SymphonyPackageLoader), "�^�C���A�E�g���܂���", "OK");
            }

            if (listRequest.Status == StatusCode.Failure)
            {
                Debug.LogError("Failed to fetch package list: " + listRequest.Error.message);
                return null;
            }

            return listRequest.Result;
        }

        /// <summary>
        /// �p�b�P�[�W�����[�h����Ă��邩�`�F�b�N����
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
        /// �p�b�P�[�W�����[�h����
        /// </summary>
        /// <param name="packageNames"></param>
        /// <returns></returns>
        private static async Task InstallPackageAsync(string[] packageNames)
        {
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
            }
        }


    }
}