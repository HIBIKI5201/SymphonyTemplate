using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SymphonyFrameWork.Debugger
{
    /// <summary>
    /// �G�f�B�^�p�̃��O�𔭍s����N���X
    /// </summary>
    public static class SymphonyDebugLog
    {
#if UNITY_EDITOR
        private static string _logText = string.Empty;
#endif

        /// <summary>
        /// �G�f�B�^��ł̂ݏo�͂����f�o�b�O���O
        /// </summary>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void DirectLog(string text)
        {
#if UNITY_EDITOR
            Debug.Log(text);
#endif
        }

        /// <summary>
        /// ���O�̃e�L�X�g�Ƀ��b�Z�[�W��ǉ�����
        /// </summary>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void AddText(string text)
        {
#if UNITY_EDITOR
            _logText += $"{text}\n";
#endif
        }

        /// <summary>
        /// �ǉ����ꂽ���b�Z�[�W���폜����
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void ClearText()
        {
#if UNITY_EDITOR
            _logText = string.Empty;
#endif
        }

        /// <summary>
        /// �ǉ����ꂽ���b�Z�[�W�����O�ɏo�͂���
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void TextLog()
        {
#if UNITY_EDITOR
            Debug.Log(_logText);
#endif
        }

        /// <summary>
        /// �R���|�[�l���g�������ꍇ�Ɍx����\������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        [Conditional("UNITY_EDITOR")]
        public static void CheckComponentNull<T>(this T component) where T : Component
        {
#if UNITY_EDITOR
            if (component == null)
            {
                Debug.LogWarning($"The component {typeof(T).Name} of {component.name} is null.");
            }
#endif
        }

        [Obsolete("���̋@�\�͈��S�����ۏႳ��Ă��܂���BCheckComponentNull���g�p���Ă�������")]
        public static bool IsComponentNotNull<T>(this T component) where T : Component
        {
            if (component == null) {
                Debug.LogWarning($"The component of type {typeof(T).Name} is null.");
                return false;
            }
            else {
                return true;
            }
        }
    }
}