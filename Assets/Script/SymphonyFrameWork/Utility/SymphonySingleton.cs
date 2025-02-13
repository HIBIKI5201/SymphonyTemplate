using SymphonyFrameWork.CoreSystem;
using System;
using System.Reflection;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public class SymphonySingleton : MonoBehaviour
    {
        [SerializeField]
        Component _target;

        private void OnEnable()
        {
            if (_target)
            {
                //Targetのクラスをキャストして実行する
                Type targetType = _target.GetType();
                MethodInfo method = typeof(ServiceLocator).GetMethod(nameof(ServiceLocator.SetInstance))
                    .MakeGenericMethod(targetType);

                method.Invoke(null, new object[] { _target, ServiceLocator.LocateType.Locator });
            }
        }

        private void OnDisable()
        {
            if (_target)
            {
                Type targetType = _target.GetType();

                //ServiceLocator.DestroyInstanceを取得する
                MethodInfo destroyMethod = typeof(ServiceLocator)
                            .GetMethod("DestroyInstance",
                            BindingFlags.Public | BindingFlags.Static,
                            null, Type.EmptyTypes, null)
                            .MakeGenericMethod(targetType);

                destroyMethod.Invoke(null, null);
            }
        }
    }
}