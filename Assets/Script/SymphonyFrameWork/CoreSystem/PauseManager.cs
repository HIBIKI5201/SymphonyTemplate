using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SymphonyFrameWork.CoreSystem
{
    public static class PauseManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initiazlze()
        {
            _pause = false;
            OnPauseChanged = null;
        }

        private static bool _pause;
        public static bool Pause
        {
            get => _pause;
            set
            {
                _pause = value;
                OnPauseChanged?.Invoke(value);
            }
        }

        [Tooltip("ポーズ時にtrue、リズーム時にfalseで実行するイベント")]
        public static event Action<bool> OnPauseChanged;

        /// <summary>
        /// ポーズ時に停止するWaitForSecond
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static IEnumerator PausableWaitForSecond(float time)
        {
            while (time > 0)
            {
                if (!_pause)
                {
                    time -= Time.deltaTime;
                }
                yield return null;
            }
        }

        /// <summary>
        /// ポーズ時に停止するWaitForSecond
        /// </summary>
        /// <param name="time"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task PausableWaitForSecondAsync(float time, CancellationToken token = default)
        {
            while (time > 0)
            {
                if (!_pause)
                {
                    time -= Time.deltaTime;
                }
                await Awaitable.NextFrameAsync(token);
            }
        }

        /// <summary>
        /// ポーズ中に停止するGameObjectのDestroy
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        public static async void PausableDestroy(GameObject obj, float t, CancellationToken token = default)
        {
            await PausableWaitForSecondAsync(t, token);

            UnityEngine.Object.Destroy(obj);
        }

        public static async void PausableInvoke(Action action, float t, CancellationToken token = default)
        {
            await PausableWaitForSecondAsync(t, token);

            action?.Invoke();
        }

        public interface IPausable
        {
            private static Dictionary<IPausable, Action<bool>> PauseEventDictionary = new();

            void Pause();
            void Resume();

            /// <summary>
            /// PauseManagerにポーズ時のイベントを購買登録する
            /// </summary>
            /// <param name="pausable"></param>
            static void RegisterPauseManager(IPausable pausable)
            {
                if (PauseEventDictionary.ContainsKey(pausable))
                {
                    return;
                }

                Action<bool> pauseEvent = OnPauseEvent;

                PauseEventDictionary.Add(pausable, pauseEvent);

                OnPauseChanged += pauseEvent;

                void OnPauseEvent(bool paused)
                {
                    if (paused)
                    {
                        pausable.Pause();
                    }
                    else
                    {
                        pausable.Resume();
                    }
                }
            }

            /// <summary>
            /// ポーズ時のイベントを購買解除する
            /// </summary>
            /// <param name="pausable"></param>
            static void UnregisterPauseManager(IPausable pausable)
            {
                if (PauseEventDictionary.TryGetValue(pausable, out var pauseEvent))
                {
                    OnPauseChanged -= pauseEvent;
                    PauseEventDictionary.Remove(pausable);
                }
            }
        }
    }
}