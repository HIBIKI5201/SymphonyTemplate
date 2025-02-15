using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Attribute
{
    public class DisplayTextAttribute : PropertyAttribute
    {
        public string Text { get; private set; }

        public DisplayTextAttribute(string text)
        {
            Text = text;
        }
    }
    [CustomPropertyDrawer(typeof(DisplayTextAttribute))]
    public class DisplayTextDecoratorDrawer : DecoratorDrawer
    {
        // 属性のインスタンスを取得するためのプロパティ
        DisplayTextAttribute DisplayTextAttribute
        {
            get { return (DisplayTextAttribute)attribute; }
        }

        // DecoratorDrawer は実際のフィールドの高さとは別に、描画領域の高さを指定できる
        public override float GetHeight()
        {
            // 複数行に対応したい場合は、EditorStyles.label.CalcHeight() などで計算可能
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            position.x += position.width / 2;
            position.width /= 2;

            // 指定した領域にテキストを表示する
            EditorGUI.LabelField(position, DisplayTextAttribute.Text);
        }
    }
}
