using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork.Attribute
{
    /// <summary>
    /// �C���X�y�N�^�[�ɕ�����\������
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
    /// �����̕`����s��
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

            // �w�肵���̈�Ƀe�L�X�g��\������
            EditorGUI.LabelField(position, DisplayTextAttribute.Text, style);
        }
    }
}
