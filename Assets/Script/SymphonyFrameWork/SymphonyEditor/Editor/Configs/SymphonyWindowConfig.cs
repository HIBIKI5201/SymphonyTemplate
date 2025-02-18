using SymphonyFrameWork.Attribute;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public class SymphonyWindowConfig : ScriptableObject
    {
        [ReadOnly, SerializeField]
        private bool _autoSceneListUpdate = true;
    }
}
