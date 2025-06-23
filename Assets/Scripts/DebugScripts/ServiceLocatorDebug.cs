using System;
using SymphonyFrameWork.System;
using UnityEngine;

namespace BLINDED_AM_ME
{
    public class ServiceLocatorDebug : MonoBehaviour
    {
        private async void Start()
        {
            Debug.LogWarning(GC.GetTotalMemory(true));

            ServiceLocator.SetInstance(this);

            Debug.LogWarning(GC.GetTotalMemory(true));

        }



        private void Action() => Debug.Log("After Action called");
    }
}