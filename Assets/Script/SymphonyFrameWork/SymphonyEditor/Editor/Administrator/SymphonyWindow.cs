using System;
using System.Collections.Generic;
using System.Reflection;
using SymphonyFrameWork.Core;
using SymphonyFrameWork.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    ///     SymphonyFrameWorkの管理パネルを表示するクラス
    /// </summary>
    public class SymphonyWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Symphony Administrator";
        private const string UITK_PATH = SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor/Editor/Administrator/UITK/";

        private PauseWindow _pauseWindow;
        
        private void OnEnable()
        {
            var container = LoadWindow();

            if (container != null)
            {
                _pauseWindow = container.Q<PauseWindow>();
                LocateDictInit(container);
                SceneLoaderInit(container);
            }
            else
            {
                Debug.LogWarning("ウィンドウがロードできませんでした");
            }
            
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        private void Update()
        {
            _pauseWindow?.Update();
        }



        /// <summary>
        ///     ウィンドウ表示
        /// </summary>
        [MenuItem(SymphonyConstant.MENU_PATH + WINDOW_NAME, priority = 0)]
        public static void ShowWindow()
        {
            var wnd = GetWindow<SymphonyWindow>();
            wnd.titleContent = new GUIContent(WINDOW_NAME);
        }

        /// <summary>
        ///     UXMLを追加
        /// </summary>
        /// <returns></returns>
        private TemplateContainer LoadWindow()
        {
            rootVisualElement.Clear();

            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UITK_PATH + "SymphonyWindow.uxml");
            ;
            if (windowTree != null)
            {
                var windowElement = windowTree.Instantiate();
                rootVisualElement.Add(windowElement);
                return windowElement;
            }

            Debug.LogError("ウィンドウが見つかりません");
            return null;
        }
        

        #region ServiceLocator

        private FieldInfo locateInfo;
        private Dictionary<Type, Component> locateDict;
        private ListView locateList;

        private void LocateDictInit(VisualElement root)
        {
            locateInfo =
                typeof(ServiceLocator).GetField("_singletonObjects", BindingFlags.Static | BindingFlags.NonPublic);

            if (locateInfo != null) locateDict = (Dictionary<Type, Component>)locateInfo.GetValue(null);

            locateList = root.Q<ListView>("locate-list");

            locateList.makeItem = () => new Label();

            // 項目のバインド（データを UI に反映）
            locateList.bindItem = (element, index) =>
            {
                var kvp = GetLocateList()[index];
                (element as Label).text = $"type : {kvp.Key.Name}\nobj : {kvp.Value.name}";
            };

            // データのセット
            locateList.itemsSource = GetLocateList();

            // 選択タイプの設定
            locateList.selectionType = SelectionType.None;

            EditorApplication.update += LocateListUpdate;
        }

        private void LocateStop()
        {
            EditorApplication.update -= LocateListUpdate;
        }

        private List<KeyValuePair<Type, Component>> GetLocateList()
        {
            if (locateDict != null) locateDict = (Dictionary<Type, Component>)locateInfo.GetValue(null);
            return locateDict != null
                ? new List<KeyValuePair<Type, Component>>(locateDict)
                : new List<KeyValuePair<Type, Component>>();
        }

        private void LocateListUpdate()
        {
            if (locateList != null)
            {
                locateList.itemsSource = GetLocateList();
                locateList.Rebuild();
            }
        }

        #endregion

        #region SceneLoader

        private static Toggle _autoSceneListUpdateToggle;

        private static void SceneLoaderInit(VisualElement root)
        {
            //コンフィグデータを取得
            var config = SymphonyConfigManager.GetConfig<AutoEnumGeneratorConfig>();
            _autoSceneListUpdateToggle = root.Q<Toggle>("enum-scene");
            _autoSceneListUpdateToggle.value = config.AutoSceneListUpdate;

            //トグルが変更された時にコンフィグを更新
            _autoSceneListUpdateToggle.RegisterValueChangedCallback(
                evt => config.AutoSceneListUpdate = evt.newValue);
        }

        #endregion
    }
}