using System.Threading.Tasks;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.UIElements;
using NotImplementedException = System.NotImplementedException;

namespace SymphonyFrameWork
{
    [UxmlElement]
    public partial class ServiceLocatorWindow : SymphonyVisualElement
    {
        public ServiceLocatorWindow() : base(
            "Assets/Script/SymphonyFrameWork/SymphonyEditor/Editor/Administrator/UITK/UXML/ServiceLocatorWindow.uxml",
            initializeType: InitializeType.None,
            loadType : LoadType.AssetDataBase)
        {
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
