using SymphonyFrameWork.Attribute;
using UnityEngine;

namespace SymphonyFrameWork.Editor
{
    public class AutoEnumGeneratorConfig : ScriptableObject
    {
        [ReadOnly] [SerializeField] private bool _autoSceneListUpdate = true;

        public bool AutoSceneListUpdate
        {
            get => _autoSceneListUpdate;
            set => _autoSceneListUpdate = value;
        }
    }
}