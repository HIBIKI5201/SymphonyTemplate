using SymphonyFrameWork.CoreSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Editor
{
    public class StaticFieldEditorUI : EditorWindow
    {
        private static VisualElement ElementBase
        {
            get
            {
                var element = new VisualElement()
                {
                    style =
                    {
                        alignItems = Align.Center,
                        alignSelf = Align.Center,
                        alignContent = Align.Center,

                        top = 20,
                        bottom = 20,
                    }
                };
                return element;
            }
        }

        private static Label Title
        {
            get
            {
                var element = new Label()
                {
                    style =
                    {
                        fontSize = 20,
                        bottom = 5,
                    }
                };
                return element;
            }
        }

        [MenuItem("Symphony FrameWork/Admin")]
        public static void ShowWindow()
        {
            StaticFieldEditorUI wnd = GetWindow<StaticFieldEditorUI>();
            wnd.titleContent = new GUIContent("Symphony Admin");
        }

        private void OnEnable()
        {
            // UI を作成
            var root = rootVisualElement;
            PauseInit(root);
            LocateDictInit(root);

            EditorApplication.update += PauseVisualUpdate;
            EditorApplication.update += UpdateListUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= PauseVisualUpdate;
            EditorApplication.update -= UpdateListUpdate;
        }

        #region Update

        private void PauseVisualUpdate()
        {
            if (_pauseInfo != null)
            {
                bool active = (bool)_pauseInfo.GetValue(null);
                _pauseVisual.style.backgroundColor = active ? Color.green : Color.red;
            }
            else
            {
                _pauseVisual.style.backgroundColor = Color.red;
            }
        }
        private void UpdateListUpdate()
        {
            locateList.itemsSource = GetSceneList();
            locateList.Rebuild();
        }

        #endregion

        #region PauseManager

        private FieldInfo _pauseInfo;
        private VisualElement _pauseVisual;

        private void PauseInit(VisualElement root)
        {
            // _pause フィールドを取得
            _pauseInfo = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            VisualElement @base = ElementBase;
            @base.style.height = 100;

            // ラベル
            Label pauseTitle = Title;
            pauseTitle.text = "Pause 状態";

            @base.Add(pauseTitle);

            // Toggle (チェックボックス)
            _pauseVisual = new VisualElement()
            {
                style =
                {
                    width = 40,
                    height = 40,
                }
            };

            // 初期値を設定


            @base.Add(_pauseVisual);

            root.Add(@base);
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

            VisualElement @base = ElementBase;

            Label title = Title;
            title.text = "ロケート登録しているもの";

            @base.Add(title);

            locateList = new ListView
            {
                makeItem = () => new Label(),
                bindItem = (element, index) =>
                {
                    var kvp = GetSceneList()[index];
                    (element as Label).text = $"type : {kvp.Key.Name} -> obj : {kvp.Value.name}";
                },
                itemsSource = GetSceneList(),
                selectionType = SelectionType.None
            };
            @base.Add(locateList);

            root.Add(@base);
        }

        private List<KeyValuePair<Type, Component>> GetSceneList()
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