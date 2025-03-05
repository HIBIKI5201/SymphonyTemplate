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

        private class AudioSettingData
        {
            private AudioMixerGroup _group;
            public AudioMixerGroup Group { get => _group; }

            private AudioSource _source;
            public AudioSource Source { get => _source; }

            private float _originalVolume = 1;
            public float OriginalVolume { get => _originalVolume; }

            public AudioSettingData(AudioMixerGroup group, AudioSource source, float originalVolume)
            {
                _group = group;
                _source = source;
                _originalVolume = originalVolume;
            }
        }

        internal static void Initialize()
        {
            _instance = null;
            _config = SymphonyConfigLocator.GetConfig<AudioManagerConfig>();
            _audioDict = null;
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

            CreateInstance();

            AudioMixer mixer = _config?.AudioMixer;

            if (!mixer)
            {
                Debug.LogWarning("オーディオミキサーがアサインされていません");
                return;
            }

            foreach (AudioGroupTypeEnum type in Enum.GetValues(typeof(AudioGroupTypeEnum)))
            {
                if (type == AudioGroupTypeEnum.None) continue;

                //Enumの名前を出す
                string name = type.ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                //ミキサーグループを取得する
                AudioMixerGroup group = mixer.FindMatchingGroups(name).FirstOrDefault();
                if (group)
                {
                    AudioSource source = _instance.AddComponent<AudioSource>();
                    source.outputAudioMixerGroup = group;
                    source.playOnAwake = false;

                    //初期のボリュームを取得
                    if (mixer.GetFloat($"{name}_Volume", out float value))
                    {
                        //各情報を追加
                        _audioDict.Add(type, new AudioSettingData(group, source, value));
                    }
                    else
                    {
                        SymphonyDebugLog.AddText($"{name}_Volume is not found");
                    }
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

            if (value < 0 || 1 < value)
            {
                Debug.LogWarning("入力は無効な値です");
                return;
            }

            //デシベルで音量を割合変更
            float db = value * (_audioDict[type].OriginalVolume + 80) - 80;

            _config?.AudioMixer.SetFloat(type.ToString(), db);
        }
    }
}
