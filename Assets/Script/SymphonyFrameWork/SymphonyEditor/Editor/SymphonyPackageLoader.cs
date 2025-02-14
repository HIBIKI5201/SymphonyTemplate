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

                // �C���X�g�[������Ă���p�b�P�[�W�̖��O���擾
                var installedPackages = listRequest.Result;
                foreach (var packageName in requirePackages)
                {
                    var isPackageInstalled = installedPackages.Any(pkg => pkg.name == packageName);
                    if (!isPackageInstalled)
                    {
                        // �p�b�P�[�W���C���X�g�[������Ă��Ȃ��ꍇ�A�C���X�g�[�������݂�
                        //InstallPackage(packageName);

                        Debug.Log($"is package installed {isPackageInstalled}");
                    }
                }

                Debug.Log(string.Join(" ", installedPackages));
            };

            void InstallPackage(string name)
            {
                // �p�b�P�[�W���C���X�g�[��
                AddRequest addRequest = Client.Add(name);

                // �C���X�g�[��������̃R�[���o�b�N
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

                        // ����X�V�ł̏������~
                        EditorApplication.update -= () => { };
                    }
                };
            }
        }
    }
}
