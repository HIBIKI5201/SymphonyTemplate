using SymphonyFrameWork;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.Debugger;
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

    private SymphonyDebugHUD _debugHUD;

    private void Start()
    {
        _velocity = Vector3.up * _speed;

        AudioManager.VolumeSliderChanged(AudioGroupTypeEnum.BGM.ToString(), 1);
        _debugHUD = FindAnyObjectByType<SymphonyDebugHUD>();
    }

    private void Update()
    {
        transform.position += _velocity * Time.deltaTime;
        if (transform.position.y > 10)
        {
            transform.position = Vector3.zero;
        }

        SymphonyDebugHUD.AddText($"Current Position: {transform.position}\n" +
            $"Velocity: {_velocity}\n" +
            $"Speed: {_speed}\n" +
            $"Curve Value at 0.5: {_curve.Evaluate(0.5f)}" +
            $"current time is {Time.time}");
    }
}