using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public static class SymphonyTask
    {
        public static async void BackGroundThreadAction(Action action)
        {
            await Awaitable.BackgroundThreadAsync();
            action.Invoke();
            Debug.Log($"{action}ÇÃé¿çsÇ™èIóπÇµÇ‹ÇµÇΩ");
        }

        public static async Task WaitUntil(Func<bool> action, CancellationToken token)
        {
            while (!action.Invoke())
            {
                await Awaitable.NextFrameAsync(token);
            }
        }
    }
}