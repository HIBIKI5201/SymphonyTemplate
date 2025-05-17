using SymphonyFrameWork;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.System;
using UnityEngine;

public class DbugObj : MonoBehaviour, IGameObject
{
    [SerializeField] private float _speed = 5;

    [DisplayText("試し書き試し書き試し書き")]
    [ReadOnly]
    [SerializeField]
    private Vector3 _velocity;

    [SerializeField] private AnimationCurve _curve;

    private void Start()
    {
        _velocity = Vector3.up * _speed;

        AudioManager.VolumeSliderChanged(AudioGroupTypeEnum.BGM, 1);
    }
}