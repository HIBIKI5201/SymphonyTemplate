using SymphonyFrameWork.Debugger;
using System;
using System.Threading;
using UnityEngine;

namespace SymphonyFrameWork.Utility
{
    public static class SymphonyTween
    {
        /// <summary>
        /// 指定した時間の間、Linearな曲線で指定した範囲を毎フレーム実行します
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s">スタートの値</param>
        /// <param name="action">実行内容</param>
        /// <param name="e">エンドの値</param>
        /// <param name="d">長さ</param>
        public static async void TweeningLerp<T>(T s, Action<T> action, T e, float d,
            CancellationToken token = default) where T : struct
        {
            float timer = Time.time;

            //時間終了までループ
            while (timer + d <= Time.time)
            {
                float elapsed = Time.time - timer;

                float t = elapsed / d; //正規化された値

                T? result = LerpValue((s, e), t);

                if (result == null)
                {
                    SymphonyDebugLog.DirectLog($"{typeof(T).Name}型は{nameof(Tweening)}に対応していません");
                    return;
                }


                action?.Invoke(result.Value);

                await Awaitable.NextFrameAsync();
            }

            //最後に最終値でやる
            action?.Invoke(e);
        }

        /// <summary>
        /// 対応した型のLerpした値を返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static T? LerpValue<T>((T s, T e) value, float t) where T : struct
        {
            T? result = value switch
            {
                (int s, int e) => (T)Convert.ChangeType(Mathf.Lerp(s, e, t), typeof(T)),
                (float s, float e) => (T)Convert.ChangeType(Mathf.Lerp(s, e, t), typeof(T)),
                (Vector2 s, Vector2 e) => (T)Convert.ChangeType(Vector2.Lerp(s, e, t), typeof(T)),
                (Vector3 s, Vector3 e) => (T)Convert.ChangeType(Vector3.Lerp(s, e, t), typeof(T)),
                (Quaternion s, Quaternion e) => (T)Convert.ChangeType(Quaternion.Lerp(s, e, t), typeof(T)),
                (Color s, Color e) => (T)Convert.ChangeType(Color.Lerp(s, e, t), typeof(T)),
                _ => null
            };

            return result;
        }
    }
}
