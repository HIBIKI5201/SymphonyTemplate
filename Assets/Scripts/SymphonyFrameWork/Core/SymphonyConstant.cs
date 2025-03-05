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
            public const string ServiceLocatorGetInstanceLog = "ServiceLocatorGetInstanceLog";
#endif
        }
    }
}