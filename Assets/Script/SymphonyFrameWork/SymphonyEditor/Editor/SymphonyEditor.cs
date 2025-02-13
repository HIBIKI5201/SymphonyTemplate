using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using SymphonyFrameWork.CoreSystem;

namespace SymphonyFrameWork.Editor
{
    public class StaticFieldEditorUI : EditorWindow
    {
        private FieldInfo _pauseField;
        private VisualElement _pauseVisual;

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
                        bottom = 10,
                    }
                };
                return element;
            }
        }

        [MenuItem("Window/Static Field Editor UI")]
        public static void ShowWindow()
        {
            StaticFieldEditorUI wnd = GetWindow<StaticFieldEditorUI>();
            wnd.titleContent = new GUIContent("Static Field Editor UI");
        }

        private void OnEnable()
        {
            // `_pause` フィールドを取得
            _pauseField = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            // UI を作成
            var root = rootVisualElement;
            PauseInit(root);

            EditorApplication.update += PauseVisualUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= PauseVisualUpdate;
        }

        private void PauseInit(VisualElement root)
        {
            VisualElement pause = ElementBase;

            // ラベル
            Label pauseTitle = Title;
            pauseTitle.text = "Pause 状態";

            pause.Add(pauseTitle);

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


            pause.Add(_pauseVisual);

            root.Add(pause);
        }

        private void PauseVisualUpdate()
        {
            if (_pauseField != null)
            {
                bool active = (bool)_pauseField.GetValue(null);
                _pauseVisual.style.backgroundColor = active ? Color.green : Color.red;
            }
            else
            {
                _pauseVisual.style.backgroundColor = Color.red;
            }
        }
    }
}