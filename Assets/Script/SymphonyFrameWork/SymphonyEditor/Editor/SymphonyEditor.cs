using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using SymphonyFrameWork.CoreSystem;

namespace SymphonyFrameWork.Editor
{
    public class StaticFieldEditorUI : EditorWindow
    {
        private FieldInfo _staticField;
        private Toggle _pauseToggle;

        [MenuItem("Window/Static Field Editor UI")]
        public static void ShowWindow()
        {
            StaticFieldEditorUI wnd = GetWindow<StaticFieldEditorUI>();
            wnd.titleContent = new GUIContent("Static Field Editor UI");
        }

        private void OnEnable()
        {
            // `_pause` フィールドを取得
            _staticField = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            // UI を作成
            var root = rootVisualElement;

            VisualElement pause = new VisualElement()
            {
                style =
                {
                    alignItems = Align.Center,
                    alignSelf = Align.Center,
                    alignContent = Align.Center,
                }
            };
            // ラベル
            Label titleLabel = new Label("Pause 状態")
            {
                style =
                {
                    top = 10
                }
            };
            pause.Add(titleLabel);

            // Toggle (チェックボックス)
            _pauseToggle = new Toggle("Pause")
            {
                style =
                {
                    marginTop = 10,
                }
            };

            // 初期値を設定
            if (_staticField != null)
            {
                _pauseToggle.value = (bool)_staticField.GetValue(null);
            }

            // 値が変更されたときの処理
            _pauseToggle.RegisterValueChangedCallback(evt =>
            {
                if (_staticField != null)
                {
                    _staticField.SetValue(null, evt.newValue);
                }
            });

           pause.Add(_pauseToggle);
           
           root.Add(pause);
        }
    }
}