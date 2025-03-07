using UnityEngine;

public class SMBTransmitter : StateMachineBehaviour
{
    private SMBReceiver _receiver = default;

    [SerializeField]
    private string _stateName = string.Empty;

    [SerializeField]
    private bool _enter;
    [SerializeField]
    private bool _update;
    [SerializeField]
    private bool _exit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_enter)
        {
            StateEvent(animator, stateInfo, SMBReceiver.Phase.Enter);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_update)
        {
            StateEvent(animator, stateInfo, SMBReceiver.Phase.Update);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_exit)
        {
            StateEvent(animator, stateInfo, SMBReceiver.Phase.Exit);
        }
    }

    /// <summary>
    /// ステートのイベント
    /// </summary>
    private void StateEvent(Animator animator, AnimatorStateInfo info, SMBReceiver.Phase state)
    {
        if (string.IsNullOrEmpty(_stateName))
        {
            _stateName = "empty state name";
        }

        SendSignal(_stateName, state, animator);
    }

    /// <summary>
    /// Receiverにイベントを送る
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="state"></param>
    /// <param name="animator"></param>
    private void SendSignal(string stateName, SMBReceiver.Phase state, Animator animator)
    {
        //Receiverが無ければ取得・追加する
        if (!_receiver)
        {
            _receiver = animator.GetComponent<SMBReceiver>();
            if (!_receiver)
            {
                _receiver = animator.gameObject.AddComponent<SMBReceiver>();
            }
        }

        _receiver.ReceiveSignal(stateName, state);
    }
}
