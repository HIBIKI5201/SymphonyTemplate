using System;
using System.Reflection;
using SymphonyFrameWork.System;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    /// <summary>
    ///     ServiceLocatorにロケート登録するクラス
    /// </summary>
    [HelpURL("https://www.notion.so/SymphonyLocate-19d7c2c6cc02809ea815c3a750fa95ca?pvs=4")]
    public class SymphonyLocate : MonoBehaviour
    {
        [SerializeField] [Tooltip("ロケートするコンポーネント")]
        private Component _target;

        private void OnEnable()
        {
            if (_target)
            {
                //Targetのクラスをキャストして実行する
                var targetType = _target.GetType();
                var method = typeof(ServiceLocator)
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
                var targetType = _target.GetType();

                //ServiceLocator.DestroyInstanceを取得する
                var destroyMethod = typeof(ServiceLocator)
                    .GetMethod(nameof(ServiceLocator.DestroyInstance),
                        BindingFlags.Public | BindingFlags.Static,
                        null, Type.EmptyTypes, null)
                    .MakeGenericMethod(targetType);

                destroyMethod.Invoke(null, null);
            }
        }
    }
}