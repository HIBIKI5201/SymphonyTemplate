using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

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
            private string _audioGroupName = string.Empty;
            public string AudioGroupName { get => _audioGroupName; }

            [SerializeField]
            private string _audioGroupVolumeParameter = string.Empty;
            public string AudioGroupVolumeParameter { get => _audioGroupVolumeParameter; }

            [SerializeField]
            private bool _isLoop = false;
            public bool IsLoop { get => _isLoop; }
        }
    }
}
