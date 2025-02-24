using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SymphonyFrameWork.System;
using SymphonyFrameWork.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    [UxmlElement]
    public partial class ServiceLocatorWindow : SymphonyVisualElement
    {
        private Dictionary<Type, Component> _locateDict;
        private FieldInfo _locateInfo;
        private ListView _locateList;

        public ServiceLocatorWindow() : base(
            "Assets/Script/SymphonyFrameWork/SymphonyEditor/Editor/Administrator/UITK/UXML/ServiceLocatorWindow.uxml",
            InitializeType.None,
            LoadType.AssetDataBase)
        {
        }

        protected override Task Initialize_S(TemplateContainer container)
        {
            _locateInfo =
                typeof(ServiceLocator).GetField("_singletonObjects", BindingFlags.Static | BindingFlags.NonPublic);

            if (_locateInfo != null) _locateDict = (Dictionary<Type, Component>)_locateInfo.GetValue(null);

            _locateList = container.Q<ListView>("locate-list");

            _locateList.makeItem = () => new Label();

            // 項目のバインド（データを UI に反映）
            _locateList.bindItem = (element, index) =>
            {
                var kvp = GetLocateList()[index];
                (element as Label).text = $"type : {kvp.Key.Name}\nobj : {kvp.Value.name}";
            };

            // データのセット
            _locateList.itemsSource = GetLocateList();

            // 選択タイプの設定
            _locateList.selectionType = SelectionType.None;

            return Task.CompletedTask;
        }

        private List<KeyValuePair<Type, Component>> GetLocateList()
        {
            if (_locateDict != null) _locateDict = (Dictionary<Type, Component>)_locateInfo.GetValue(null);
            return _locateDict != null
                ? new List<KeyValuePair<Type, Component>>(_locateDict)
                : new List<KeyValuePair<Type, Component>>();
        }

        public void Update()
        {
            if (_locateList != null)
            {
                _locateList.itemsSource = GetLocateList();
                _locateList.Rebuild();
            }
        }
    }
}