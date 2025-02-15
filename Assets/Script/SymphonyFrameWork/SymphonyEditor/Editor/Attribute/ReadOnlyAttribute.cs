using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SymphonyFrameWork
{
    /// <summary>
    /// �C���X�y�N�^�[��ŕҏW�s�̃v���p�e�B�𐶐�����
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }

    /// <summary>
    /// �v���p�e�B��ύX�s�ɂ���
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    /// <summary>
    /// ReadOnry���\������Ȃ��ꍇ�Ɍx�����o��
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ReadOnlyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var targetType = target.GetType();
            var fields = targetType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(ReadOnlyAttribute), true))
                {
                    var property = serializedObject.FindProperty(field.Name);
                    if (property == null)
                    {
                        Debug.LogWarning($"�t�B�[���h '{field.Name}' �� [ReadOnly] �������t�^����Ă��܂����A[SerializeField] �������t�^����Ă��Ȃ����߁A�C���X�y�N�^�[�ɕ\������܂���B");
                    }
                }
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}