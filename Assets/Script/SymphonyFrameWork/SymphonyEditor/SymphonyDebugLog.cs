using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// エディタ用のログを発行するクラス
    /// </summary>
    public static class SymphonyDebugLog
    {
#if UNITY_EDITOR
        private static string _logText = string.Empty;
#endif
        [Conditional("UNITY_EDITOR")]
        public static void DirectLog(string text)
        {
#if UNITY_EDITOR
            Debug.Log(text);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void AddText(string text)
        {
#if UNITY_EDITOR
            _logText += $"{text}\n";
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearText()
        {
#if UNITY_EDITOR
            _logText = string.Empty;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void TextLog()
        {
#if UNITY_EDITOR
            Debug.Log(_logText);
#endif
        }

        public static bool IsComponentNotNull<T>(this T component) where T : Component
        {
            if (component == null) {
                Debug.LogError($"The component of type {typeof(T).Name} is null.");
                return false;
            }
            else {
                return true;
            }
        }
    }
}