using System;
using UnityEngine;

public class SMBReceiver : MonoBehaviour
{
    public event Action<string, Phase> OnREceiveSignal;

    public void ReceiveSignal(string name, Phase phase)
    {
        Debug.Log($"name : {name}, phase : {phase}");
        OnREceiveSignal?.Invoke(name, phase);
    }

    public enum Phase
    {
        None,
        Enter,
        Update,
        Exit,
    }
}
