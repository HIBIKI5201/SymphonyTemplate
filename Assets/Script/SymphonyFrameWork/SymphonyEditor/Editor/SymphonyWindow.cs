using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    /// <summary>
    /// SymphonyFrameWorkの管理パネルを表示するクラス
    /// </summary>
    public class SymphonyWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Symphony Administrator";
        private const string UITK_PATH = SymphonyConstant.FRAMEWORK_PATH + "/SymphonyEditor/Editor/UITK/";

        /// <summary>
        /// ウィンドウ表示
        /// </summary>
        [MenuItem(SymphonyConstant.MENU_PATH + WINDOW_NAME, priority = 0)]
        public static void ShowWindow()
        {
            SymphonyWindow wnd = GetWindow<SymphonyWindow>();
            wnd.titleContent = new GUIContent(WINDOW_NAME);
        }

        private void OnEnable()
        {
            var container = LoadWindow();

            if (container != null)
            {
                PauseInit(container);
                LocateDictInit(container);
            }
            else
            {
                Debug.LogWarning("ウィンドウがロードできませんでした");
            }
        }

        private void OnDisable()
        {
            PauseStop();
            LocateStop();
        }

        /// <summary>
        /// UXMLを追加
        /// </summary>
        /// <returns></returns>
        private TemplateContainer LoadWindow()
        {
            rootVisualElement.Clear();

            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UITK_PATH + "SymphonyWindow.uxml"); ;
            if (windowTree != null)
            {
                var windowElement = windowTree.Instantiate();
                rootVisualElement.Add(windowElement);
                return windowElement;
            }
            else
            {
                Debug.LogError("ウィンドウが見つかりません");
                return null;
            }
        }

        #region PauseManager

        private FieldInfo _pauseInfo;
        private VisualElement _pauseVisual;
        private Label _pauseText;

        private void PauseInit(VisualElement root)
        {
            // _pause フィールドを取得
            _pauseInfo = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            if (root == null)
            {
                root = LoadWindow();
            }

            _pauseVisual = root.Q<VisualElement>("pause");
            _pauseText = root.Q<Label>("pause-text");

            root.Q<Button>("button-pause").clicked += () => PauseManager.Pause = true;
            root.Q<Button>("button-resume").clicked += () => PauseManager.Pause = false;

            EditorApplication.update += PauseVisualUpdate;
        }

        private void PauseStop()
        {

            EditorApplication.update -= PauseVisualUpdate;
        }


        private void PauseVisualUpdate()
        {
            if (_pauseVisual != null && _pauseInfo != null)
            {
                bool active = (bool)_pauseInfo.GetValue(null);
                _pauseVisual.style.backgroundColor = new StyleColor(active ? Color.green : Color.red);
                _pauseText.text = active ? "True" : "False";
            }
            else
            {
                _pauseVisual.style.backgroundColor = new StyleColor(Color.red);
            }
        }

        #endregion

        #region ServiceLocator

        private FieldInfo locateInfo;
        private Dictionary<Type, Component> locateDict;
        private ListView locateList;

        private void LocateDictInit(VisualElement root)
        {
            locateInfo = typeof(ServiceLocator).GetField("_singletonObjects", BindingFlags.Static | BindingFlags.NonPublic);

            if (locateInfo != null)
            {
                locateDict = (Dictionary<Type, Component>)locateInfo.GetValue(null);
            }

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
            if (locateDict != null)
            {
                locateDict = (Dictionary<Type, Component>)locateInfo.GetValue(null);
            }
            return locateDict != null ?
                new List<KeyValuePair<Type, Component>>(locateDict) :
                new List<KeyValuePair<Type, Component>>();
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
    }
}