using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    public class SymphonyWindow : EditorWindow
    {
        private const string WindowName = "Symphony Admin";
        private const string DIRECTORY_PATH = "Assets/Script/SymphonyFrameWork/SymphonyEditor/Editor/UITK/";

        /// <summary>
        /// ウィンドウ表示
        /// </summary>
        [MenuItem("Window/Symphony FrameWork/" + WindowName)]
        public static void ShowWindow()
        {
            SymphonyWindow wnd = GetWindow<SymphonyWindow>();
            wnd.titleContent = new GUIContent(WindowName);
        }

        private void OnEnable()
        {
            var container = LoadWindow();

            if (container != null)
            {
                PauseInit(container);
                LocateDictInit(container);

                EditorApplication.update += PauseVisualUpdate;
                EditorApplication.update += LocateListUpdate;
            }
            else
            {
                Debug.LogWarning("ウィンドウがロードできませんでした");
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= PauseVisualUpdate;
            EditorApplication.update -= LocateListUpdate;
        }

        /// <summary>
        /// UXMLを追加
        /// </summary>
        /// <returns></returns>
        private TemplateContainer LoadWindow()
        {
            rootVisualElement.Clear();

            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DIRECTORY_PATH + "SymphonyWindow.uxml"); ;
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

        #region Update

        private void PauseVisualUpdate()
        {
            if (_pauseInfo != null)
            {
                bool active = (bool)_pauseInfo.GetValue(null);
                _pauseVisual.style.backgroundColor = new StyleColor(active ? Color.green : Color.red);
            }
            else
            {
                _pauseVisual.style.backgroundColor = new StyleColor(Color.red);
            }
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

        #region PauseManager

        private FieldInfo _pauseInfo;
        private VisualElement _pauseVisual;

        private void PauseInit(VisualElement root)
        {
            // _pause フィールドを取得
            _pauseInfo = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            if (root == null)
            {
                root = LoadWindow();
            }

            _pauseVisual = root.Q<VisualElement>("pause");
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

            locateList.makeItem =() => new Label();

            // 項目のバインド（データを UI に反映）
            locateList.bindItem = (element, index) =>
            {
                var kvp = GetLocateList()[index];
                (element as Label).text = $"type : {kvp.Key.Name} -> obj : {kvp.Value.name}";
            };

            // データのセット
            locateList.itemsSource = GetLocateList();

            // 選択タイプの設定
            locateList.selectionType = SelectionType.None;
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

        #endregion
    }
}