namespace SymphonyFrameWork.Core
{
    public static class EditorSymphonyConstrant
    {
#if UNITY_EDITOR
        public static string RESOURCES_EDITOR_PATH = "Editor/" + SymphonyConstant.SYMPHONYFRAMEWORK + "/Config";

        public static string UITK_PATH = SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor/Editor/Administrator/UITK/";

        #region ウィンドウのコンフィグ
        public const string ServiceLocatorSetInstanceKey = "ServiceLocatorSetInstanceLog";
        public const bool ServiceLocatorSetInstanceDefault = true;
        public const string ServiceLocatorGetInstanceKey = "ServiceLocatorGetInstanceLog";
        public const bool ServiceLocatorGetInstanceDefault = false;
        public const string ServiceLocatorDestroyInstanceKey = "ServiceLocatorDestroyInstanceLog";
        public const bool ServiceLocatorDestroyInstanceDefault = true;
        #endregion

        #region Enumの名前
        public const string SceneListEnumFileName = "SceneList";
        public const string TagsEnumFileName = "Tags";
        public const string LayersEnumFileName = "Layers";
        #endregion
#endif
    }
}