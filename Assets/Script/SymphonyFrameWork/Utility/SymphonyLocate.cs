using SymphonyFrameWork.CoreSystem;
using System;
using System.Reflection;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    /// <summary>
    /// ServiceLocator�Ƀ��P�[�g�o�^����N���X
    /// </summary>
    public class SymphonyLocate : MonoBehaviour
    {
        [SerializeField, Tooltip("���P�[�g����R���|�[�l���g")]
        Component _target;

        private void OnEnable()
        {
            if (_target)
            {
                //Target�̃N���X���L���X�g���Ď��s����
                Type targetType = _target.GetType();
                MethodInfo method = typeof(ServiceLocator)
                    .GetMethod(nameof(ServiceLocator.SetInstance))
                    .MakeGenericMethod(targetType);

                method.Invoke(null, new object[]
                { _target, ServiceLocator.LocateType.Locator });
            }
        }

        private void OnDisable()
        {
            if (_target)
            {
                Type targetType = _target.GetType();

                //ServiceLocator.DestroyInstance���擾����
                MethodInfo destroyMethod = typeof(ServiceLocator)
                            .GetMethod(nameof(ServiceLocator.DestroyInstance),
                            BindingFlags.Public | BindingFlags.Static,
                            null, Type.EmptyTypes, null)
                            .MakeGenericMethod(targetType);

                destroyMethod.Invoke(null, null);
            }
        }
    }
}