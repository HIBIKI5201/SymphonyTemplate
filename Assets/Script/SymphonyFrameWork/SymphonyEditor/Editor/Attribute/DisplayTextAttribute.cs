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
        // �����̃C���X�^���X���擾���邽�߂̃v���p�e�B
        DisplayTextAttribute DisplayTextAttribute
        {
            get { return (DisplayTextAttribute)attribute; }
        }

        // DecoratorDrawer �͎��ۂ̃t�B�[���h�̍����Ƃ͕ʂɁA�`��̈�̍������w��ł���
        public override float GetHeight()
        {
            // �����s�ɑΉ��������ꍇ�́AEditorStyles.label.CalcHeight() �ȂǂŌv�Z�\
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position)
        {
            position.x += position.width / 2;
            position.width /= 2;

            // �w�肵���̈�Ƀe�L�X�g��\������
            EditorGUI.LabelField(position, DisplayTextAttribute.Text);
        }
    }
}
