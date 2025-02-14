using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SymphonyFrameWork.Debugger
{
    /// <summary>
    /// �X�g�b�v�E�H�b�`��񋟂���N���X
    /// </summary>
    public static class SymphonyStopWatch
    {
#if UNITY_EDITOR
        private static Dictionary<int, (Stopwatch watch, string text)> dict = new();
#endif
        /// <summary>
        /// �w�肳�ꂽID�̃X�g�b�v�E�H�b�`���v���J�n����
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        [Conditional("UNITY_EDITOR")]
        public static void Start(int id, string text = "time is")
        {
#if UNITY_EDITOR
            if (!dict.TryAdd(id, (Stopwatch.StartNew(), text)))
            {
                Debug.LogWarning($"�X�g�b�v�E�H�b�`��ID������Ă��܂�\n{id} �ł͂Ȃ��ʂ�ID���w�肵�Ă�������");
            }
#endif
        }

        /// <summary>
        /// ID�̃^�C�}�[���~�����O�ɏo��
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
            else Debug.LogWarning($"{id}�̃X�g�b�v�E�H�b�`�͊J�n����Ă��܂���");
#endif
        }
    }
}