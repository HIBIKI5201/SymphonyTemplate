using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.CoreSystem;
using SymphonyFrameWork.Utility;
using UnityEngine;

public class DbugObj : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;

    [DisplayText("試し書き試し書き試し書き")]

    [ReadOnly, SerializeField]
    private Vector3 _velocity = default;

    [SerializeField]
    private AnimationCurve _curve = null;

    private void Start()
    {
        _velocity = Vector3.up * _speed;

        

        SymphonyTween.Tweening(0, x => Debug.Log(x), 10, 1, _curve);
    }

    void Update()
    {
        if (PauseManager.Pause)
        {
            return;
        }

        //if (-5 > transform.position.y)
        //{
        //    _velocity = Vector3.up * _speed;
        //}

        //if (5 < transform.position.y)
        //{
        //    _velocity = Vector3.down * _speed;
        //}

        //transform.position += _velocity * Time.deltaTime;
    }
}
