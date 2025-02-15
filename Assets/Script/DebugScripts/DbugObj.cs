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

    private void Start()
    {
        _velocity = Vector3.up * _speed;
    }

    void Update()
    {
        if (PauseManager.Pause)
        {
            return;
        }

        SymphonyTween.TweeningLerp((float)0,
            x => transform.position = new Vector3(x, transform.position.y, transform.position.z), 10, 10);

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
