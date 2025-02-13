using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using SymphonyFrameWork.CoreSystem;

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
            var staticField = typeof(PauseManager).GetField("_pause", BindingFlags.Static | BindingFlags.NonPublic);

            // UI を作成
            var root = rootVisualElement;

            VisualElement pause = ElementBase;

            // ラベル
            Label pauseTitle = Title;
            pauseTitle.text = "Pause 状態";
            
            pause.Add(pauseTitle);

            // Toggle (チェックボックス)
            var pauseVisual = new VisualElement()
            {
                style =
                {
                    width = 40,
                    height = 40,
                }
            };
            
            // 初期値を設定
            if (staticField != null)
            {
                bool active = (bool)staticField.GetValue(null);
                pauseVisual.style.backgroundColor = active ? Color.green : Color.red;
            }
            else
            {
                pauseVisual.style.backgroundColor = Color.red;
            }

            pause.Add(pauseVisual);
           
            root.Add(pause);
        }
    }
}