using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    /// <summary>
    /// Task�̋@�\���g������N���X
    /// </summary>
    public static class SymphonyTask
    {
        /// <summary>
        /// �o�b�O�O���E���h�ŏ�������
        /// </summary>
        /// <param name="action"></param>
        public static async void BackGroundThreadAction(Action action)
        {
            await Awaitable.BackgroundThreadAsync();
            action.Invoke();
            Debug.Log($"{action.Method} is done");
        }

        /// <summary>
        /// �o�b�O�O���E���h�ŏ�������
        /// </summary>
        /// <param name="action"></param>
        public static async Task BackGroundThreadActionAsync(Action action)
        {
            await Awaitable.BackgroundThreadAsync();
            action.Invoke();
            Debug.Log($"{action.Method} is done");

            return;
        }

        /// <summary>
        /// ������true�ɂȂ�܂őҋ@����
        /// </summary>
        /// <param name="action">�����̌��ʂ�Ԃ����\�b�h</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> action, CancellationToken token = default)
        {
            while (!action.Invoke())
            {
                await Awaitable.NextFrameAsync(token);
            }
        }
    }
}