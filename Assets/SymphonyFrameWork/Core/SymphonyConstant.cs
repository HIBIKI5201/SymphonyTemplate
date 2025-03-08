using System.Runtime.CompilerServices;

namespace SymphonyFrameWork.Core
{
    public static class SymphonyConstant
    {
        public const string SYMPHONY_PACKAGE = "symphony.framework";
        public const string SYMPHONY_FRAMEWORK = "SymphonyFrameWork";

        /// <summary>
        /// アセットかパッケージのルートパスを返す
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        public static string FRAMEWORK_PATH([CallerFilePath] string sourceFilePath = "")
        {
            if (sourceFilePath[0] == '.')
            {
                return "Packages/" + SYMPHONY_PACKAGE;
            }
            else
            {
                return "Assets/" + SYMPHONY_FRAMEWORK;
            }
        }

        #region 自動生成物のパス
        public const string RESOURCES_RUNTIME_PATH = "Assets/Resources/" + SYMPHONY_FRAMEWORK;
        public const string ENUM_PATH = "Assets/Scripts/" + SYMPHONY_FRAMEWORK + "/Enum";
        #endregion

        public const string MENU_PATH = "Window/Symphony FrameWork/";
    }
}