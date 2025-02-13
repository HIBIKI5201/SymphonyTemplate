using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SymphonyFrameWork.Editor
{
    public static class SymphonyStopWatch
    {
#if UNITY_EDITOR
        private static Dictionary<int, (Stopwatch watch, string text)> dict = new();
#endif
        /// <summary>
        /// 他と被らないIDを指定してください
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void Start(int id, string text = "time is")
        {
#if UNITY_EDITOR
            if (!dict.TryAdd(id, (Stopwatch.StartNew(), text)))
            {
                Debug.LogWarning($"ストップウォッチのIDが被っています\n{id}ではない別のIDを指定してください");
            }
#endif
        }

        /// <summary>
        /// 開始時と同じIDを入れて下さい
        /// </summary>
        /// <param name="id"></param>
        [Conditional("UNITY_EDITOR")]
        public static void Stop(int id)
        {
#if UNITY_EDITOR
            if (dict.TryGetValue(id, out var value))
            {
                value.watch.Stop();
                Debug.Log($"{value.text} <color=green><b>{value.watch.ElapsedMilliseconds}</b></color> ms");
                dict.Remove(id);
            }
            else Debug.LogWarning($"{id}のストップウォッチは開始されていません");
#endif
        }
    }
}