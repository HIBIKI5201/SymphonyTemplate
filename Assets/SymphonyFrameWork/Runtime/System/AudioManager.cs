using SymphonyFrameWork.Config;
using SymphonyFrameWork.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SymphonyFrameWork.System
{
    /// <summary>
    /// オーディオ再生
    /// </summary>
    public static class AudioManager
    {
        private static AudioManagerConfig _config;
        private static GameObject _instance;

        private static
            Dictionary<AudioGroupTypeEnum, AudioSettingData> _audioDict = new();

        private struct AudioSettingData
        {
            public readonly AudioMixerGroup Group;
            public readonly AudioSource Source;
            public readonly string ExposedName;
            public readonly float? OriginalVolume;

            public AudioSettingData(AudioMixerGroup group, AudioSource source, string exposedName, float? originalVolume)
            {
                Group = group;
                Source = source;
                ExposedName = exposedName;
                OriginalVolume = originalVolume;
            }
        }

        internal static void Initialize()
        {
            _instance = null;
            _audioDict = null;
            _config = SymphonyConfigLocator.GetConfig<AudioManagerConfig>();
        }

        private static void CreateInstance()
        {
            if (_instance is not null) return;

            var instance = new GameObject("AudioManager");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }

        private static void AudioSourceInitialize()
        {
            if (_audioDict != null)
            {
                return;
            }

            _audioDict = new();

            CreateInstance();

            AudioMixer mixer = _config?.AudioMixer;

            if (!mixer)
            {
                Debug.LogWarning("オーディオミキサーがアサインされていません");
                return;
            }

            SymphonyDebugLog.AddText("Audio Managerを初期化しました。");

            foreach (AudioGroupTypeEnum type in Enum.GetValues(typeof(AudioGroupTypeEnum)))
            {
                if (type == AudioGroupTypeEnum.None) continue;

                //Enumの名前を出す
                string name = type.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                //Enum名からデータを取得
                var data = _config.AudioGroupSettingList.Find(s => s.AudioGroupName == name);

                if (data == null)
                {
                    Debug.LogWarning($"{name}のデータがありません。");
                    continue;
                }

                //ミキサーグループを取得する
                AudioMixerGroup group = mixer.FindMatchingGroups(name).FirstOrDefault();
                if (group)
                {
                    AudioSource source = _instance.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = group;
                    source.playOnAwake = false;
                    if (data.IsLoop) source.loop = true;

                    //初期のボリュームを取得
                    float? volume = null;
                    if (!string.IsNullOrEmpty(data.ExposedParameterName) &&
                        mixer.GetFloat(data.ExposedParameterName, out var value))
                    {
                        volume = value;
                        SymphonyDebugLog.AddText($"{name}は正常に追加されました。volume : {volume}");
                    }
                    else
                    {
                        SymphonyDebugLog.AddText($"{name}のVolumeParameterが見つかりませんでした");
                    }

                    //各情報を追加
                    _audioDict.Add(type, new AudioSettingData(group, source, data.ExposedParameterName, volume ?? 0));
                }
                else
                {
                    SymphonyDebugLog.AddText($"{name} is not a valid AudioMixerGroup.");
                }
            }

            SymphonyDebugLog.TextLog();
        }

        public static void VolumeSliderChanged(AudioGroupTypeEnum type, float value)
        {
            AudioSourceInitialize();

            if (type == AudioGroupTypeEnum.None)
            {
                return;
            }

            if (value < 0 || 1 < value)
            {
                Debug.LogWarning("入力は無効な値です");
                return;
            }

            if (!_audioDict.TryGetValue(type, out var data)) return;

            if (data.OriginalVolume == null)
            {
                Debug.LogWarning($"{type}のボリュームがありません");
                return;
            }

            //デシベルで音量を割合変更
            float db = value * (data.OriginalVolume.Value + 80) - 80;

            _config?.AudioMixer.SetFloat(data.ExposedName, db);
        }

        public static AudioSource GetAudioSource(AudioGroupTypeEnum type)
        {
            AudioSourceInitialize();

            if (type == AudioGroupTypeEnum.None)
            {
                return null;
            }

            return _audioDict.TryGetValue(type, out var data) ? data.Source : null;
        }
    }
}
