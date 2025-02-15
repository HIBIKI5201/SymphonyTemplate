using SymphonyFrameWork;
using SymphonyFrameWork.CoreSystem;
using UnityEngine;

public class DbugObj : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5;

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

        if (-5 > transform.position.y)
        {
            _velocity = Vector3.up * _speed;
        }

        if (5 < transform.position.y)
        {
            _velocity = Vector3.down * _speed;
        }

        transform.position += _velocity * Time.deltaTime;
    }
}
