namespace SymphonyFrameWork.Core
{
    public static class SymphonyConstant
    {
        public const string FRAMEWORK_PATH = "Assets/Scripts/SymphonyFrameWork";

        public const string RESOURCES_RUNTIME_PATH = FRAMEWORK_PATH + "/Runtime/Resources";
        public const string RESOURCES_EDITOR_PATH = FRAMEWORK_PATH + "/SymphonyEditor/Editor/Resources";

        public const string ENUM_PATH = FRAMEWORK_PATH + "/Runtime/Enum";

        public const string MENU_PATH = "Window/Symphony FrameWork/";


        public class EditorSymphonyConstrant
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
}