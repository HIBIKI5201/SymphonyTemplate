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
    public class DisplayTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var displayTextAttribute = (DisplayTextAttribute)attribute;
            EditorGUI.LabelField(position, displayTextAttribute.Text);
        }
    }
}
