using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// �V���O���g���̃C���X�^���X�𓝊����ĊǗ�����N���X
    /// </summary>
    //�C���X�^���X���ꎞ�I�ɃV�[�����[�h����؂藣���������ɂ��g�p�ł���
    public static class ServiceLocator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _instance = null;
            _singletonObjects.Clear();
        }

        [Tooltip("�V���O���g��������C���X�^���X�̃R���e�i")]
        private static GameObject _instance;
        [Tooltip("�V���O���g���o�^����Ă���^�̃C���X�^���X����")]
        private static Dictionary<Type, Component> _singletonObjects = new();

        /// <summary>
        /// �C���X�^���X�R���e�i�������ꍇ�ɐ�������
        /// </summary>
        private static void CreateInstance()
        {
            if (_instance is not null)
            {
                return;
            }

            GameObject instance = new GameObject("ServiceLocator");

            SymphonyCoreSystem.MoveObjectToSymphonySystem(instance);
            _instance = instance;
        }

        /// <summary>
        /// �����ꂽ�R���|�[�l���g�����P�[�^�[�ɓo�^����
        /// </summary>
        /// <typeparam name="T">�o�^����^</typeparam>
        /// <param name="instance">�C���X�^���X</param>
        /// <returns>�o�^������������true�A���s������false</returns>
        public static void SetInstance<T>(T instance, LocateType type = LocateType.Locator) where T : Component
        {
            // ���ɓo�^����Ă���ꍇ�͒ǉ��ł��Ȃ�
            if (!_singletonObjects.TryAdd(typeof(T), instance))
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            Debug.Log($"{typeof(T).Name}�N���X��{instance.name}��" +
                $"{type switch { LocateType.Locator => "���P�[�g", LocateType.Singleton => "�V���O���g��", _ => string.Empty }}�o�^����܂���");

            if (type == LocateType.Singleton)
            {
                CreateInstance();
                instance.transform.SetParent(_instance.transform);
            }
        }

        /// <summary>
        /// �w�肵���C���X�^���X��j������
        /// </summary>
        /// <typeparam name="T">�j���������C���X�^���X�̌^</typeparam>
        public static void DestroyInstance<T>(T instance) where T : Component
        {
            //�C���X�^���X���o�^���ꂽ�R���|�[�l���g��
            if (_singletonObjects.TryGetValue(typeof(T), out Component md) && md == instance)
            {
                DestroyInstance<T>();
            }
        }

        /// <summary>
        /// �w�肵���^�̃C���X�^���X��j������
        /// </summary>
        /// <typeparam name="T">�j���������C���X�^���X�̌^</typeparam>
        public static void DestroyInstance<T>() where T : Component
        {
            if (_singletonObjects.TryGetValue(typeof(T), out Component md))
            {
                Object.Destroy(md.gameObject);
                _singletonObjects.Remove(typeof(T));
                Debug.Log($"{typeof(T).Name}���j������܂���");
            }
            else
            {
                Debug.Log($"{typeof(T).Name}�͓o�^����Ă��܂���");
            }
        }

        /// <summary>
        /// �o�^���ꂽ�C���X�^���X��Ԃ�
        /// </summary>
        /// <typeparam name="T">�擾�������C���X�^���X�̌^</typeparam>
        /// <returns>�w�肵���^�̃C���X�^���X</returns>
        public static T GetInstance<T>() where T : Component
        {
            if (_singletonObjects.TryGetValue(typeof(T), out Component md))
            {
                if (md != null)
                {
                    return md as T;
                }
                else
                {
                    Debug.LogError($"{typeof(T).Name} �͔j������Ă��܂��B");
                    return null;
                }
            }

            Debug.LogWarning($"{typeof(T).Name} �͓o�^����Ă��܂���B");
            return null;
        }

        public enum LocateType
        {
            Singleton,
            Locator,
        }
    }
}