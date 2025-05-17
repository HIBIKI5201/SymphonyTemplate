using System;
using SymphonyFrameWork.System;
using UnityEngine;

namespace BLINDED_AM_ME
{
    public class ServiceLocatorDebug : MonoBehaviour
    {
        private async void Start()
        {
            ServiceLocator.RegisterAfterLocate<ServiceLocatorDebug>(Action);

            await Awaitable.WaitForSecondsAsync(1);
            
            ServiceLocator.SetInstance(this);
            
            await Awaitable.WaitForSecondsAsync(1);
            
            ServiceLocator.RegisterAfterLocate<ServiceLocatorDebug>(Action);
        }
        
        
        
        private void Action() => Debug.Log("After Action called");
    }
}