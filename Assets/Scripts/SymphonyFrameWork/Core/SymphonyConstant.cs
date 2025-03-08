namespace SymphonyFrameWork.Core
{
    public static class SymphonyConstant
    {
        public const string SYMPHONYFRAMEWORK = "SymphonyFrameWork";
        public static string FRAMEWORK_PATH
        {
            get
            {
                return "Assets/Scripts/" + SYMPHONYFRAMEWORK;
            }
        }

        #region 自動生成物のパス
        public const string RESOURCES_RUNTIME_PATH = "Assets/Resources/" + SYMPHONYFRAMEWORK;
        public const string ENUM_PATH = "Assets/Script/" + SYMPHONYFRAMEWORK + "/Enum";
        #endregion

        public const string MENU_PATH = "Window/Symphony FrameWork/";
    }
}