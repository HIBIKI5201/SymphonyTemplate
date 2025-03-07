namespace SymphonyFrameWork.Core
{
    public static class EditorSymphonyConstrant
    {
#if UNITY_EDITOR
        public const string ServiceLocatorSetInstanceKey = "ServiceLocatorSetInstanceLog";
        public const bool ServiceLocatorSetInstanceDefault = true;
        public const string ServiceLocatorGetInstanceKey = "ServiceLocatorGetInstanceLog";
        public const bool ServiceLocatorGetInstanceDefault = false;
        public const string ServiceLocatorDestroyInstanceKey = "ServiceLocatorDestroyInstanceLog";
        public const bool ServiceLocatorDestroyInstanceDefault = true;

        public const string SceneListEnumFileName = "SceneList";
        public const string TagsEnumFileName = "Tags";
        public const string LayersEnumFileName = "Layers";
#endif
    }
}