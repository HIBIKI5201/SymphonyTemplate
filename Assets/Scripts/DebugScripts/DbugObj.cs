using SymphonyFrameWork;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using UnityEngine;

public class DbugObj : MonoBehaviour, IGameObject
{
    [SerializeField] private float _speed = 5;

    [DisplayText("試し書き試し書き試し書き")] [ReadOnly] [SerializeField]
    private Vector3 _velocity;

    [SerializeField] private AnimationCurve _curve;

    private void Start()
    {
        _velocity = Vector3.up * _speed;

        var renderer = GetComponent<MeshRenderer>();
        SymphonyTween.PausableTweening(new Color(0, 0, 0), x => renderer.material.color = x, new Color(1, 1, 1), 10,
            _curve);

        ServiceLocator.GetInstance<DbugObj>();
    }

    private void Update()
    {
        if (PauseManager.Pause) return;

        if (-5 > transform.position.y) _velocity = Vector3.up * _speed;

        if (5 < transform.position.y) _velocity = Vector3.down * _speed;

        transform.position += _velocity * Time.deltaTime;
    }
}