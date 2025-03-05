using SymphonyFrameWork.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;

namespace SymphonyFrameWork.System
{
    /// <summary>
    /// オーディオ再生
    /// </summary>
    public static class AudioManager
    {
        private static GameObject _instance;

        private static
            Dictionary<AudioType, (AudioMixerGroup group, AudioSource source, float originalVolume)> _audioDict = new();
        
        private class 

        internal static void Initialize()
        {
            _instance = null;
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
            AudioMixer mixer = SymphonyConfigLocator.GetConfig<AudioManagerConfig>()?.AudioMixer;

            if (!mixer)
            {
                Debug.LogWarning("オーディオミキサーがアサインされていません");
                return;
            }

            foreach (AudioType type in Enum.GetValues(typeof(AudioType)))
            {
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
                        _audioDict.Add(type, (group, source, value));
                    }
                    else
                    {
                        Debug.LogWarning($"{name}_Volume is not found");
                    }
                }
                else
                {
                    Debug.LogWarning($"{name} is not a valid AudioMixerGroup.");
                }
            }
        }
    }
}
