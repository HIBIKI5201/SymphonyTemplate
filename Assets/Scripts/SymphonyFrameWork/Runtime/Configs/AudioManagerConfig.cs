using UnityEngine;
using UnityEngine.Audio;

namespace SymphonyFrameWork
{
    public class AudioManagerConfig : ScriptableObject
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        public AudioMixer AudioMixer { get => _audioMixer; }
    }
}
