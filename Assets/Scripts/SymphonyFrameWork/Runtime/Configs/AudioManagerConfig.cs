using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;

namespace SymphonyFrameWork
{
    public class AudioManagerConfig : ScriptableObject
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        public AudioMixer AudioMixer { get => _audioMixer; }

        [SerializeField]
        private List<AudioGroupSettings> _audioGroupSettingList;
        public List<AudioGroupSettings> AudioGroupSettingList { get => _audioGroupSettingList; }

        [Serializable]
        public class AudioGroupSettings
        {
            [SerializeField]
            private string _audioGroupName;
            public string AudioGroupName { get => _audioGroupName; }

            [SerializeField]
            private bool _isLoop;
            public bool IsLoop { get => _isLoop; }
        }
    }
}
