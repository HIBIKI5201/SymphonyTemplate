using SymphonyFrameWork.Utility;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    [UxmlElement]
    public partial class AutoEnumGeneratorWindow : SymphonyVisualElement
    {
        public AutoEnumGeneratorWindow() : base(
            SymphonyWindow.UITK_UXML_PATH + "AutoEnumGeneratorWindow.uxml",
            InitializeType.None,
            LoadType.AssetDataBase)
        { }
        protected override Task Initialize_S(TemplateContainer container)
        {
            //コンフィグデータを取得
            var config = SymphonyEditorConfigLocator.GetConfig<AutoEnumGeneratorConfig>();
            var sceneList = container.Q<Toggle>("scene");
            sceneList.value = config.AutoSceneListUpdate;

            sceneList.RegisterValueChangedCallback(
                evt => config.AutoSceneListUpdate = evt.newValue);

            return Task.CompletedTask;
        }
    }
}
