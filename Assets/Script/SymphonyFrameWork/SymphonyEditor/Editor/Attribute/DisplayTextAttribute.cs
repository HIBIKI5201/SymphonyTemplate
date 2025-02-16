using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Attribute
{
    /// <summary>
    /// インスペクターに文字を表示する
    /// </summary>
    public class DisplayTextAttribute : PropertyAttribute
    {
        public string Text { get; private set; }

        public DisplayTextAttribute(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 文字の描画を行う
    /// </summary>
    [CustomPropertyDrawer(typeof(DisplayTextAttribute))]
    public class DisplayTextDecoratorDrawer : DecoratorDrawer
    {
        DisplayTextAttribute DisplayTextAttribute
        {
            get { return (DisplayTextAttribute)attribute; }
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                fontStyle = FontStyle.Normal,
                fontSize = 12
            };

            // 指定した領域にテキストを表示する
            EditorGUI.LabelField(position, DisplayTextAttribute.Text, style);
        }
    }
}
