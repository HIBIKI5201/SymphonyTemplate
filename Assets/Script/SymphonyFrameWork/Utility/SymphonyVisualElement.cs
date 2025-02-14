using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SymphonyFrameWork.Utility
{
    /// <summary>
    /// VisualElement��CS�Ő��䂷��x�[�X�N���X
    /// </summary>
    [UxmlElement]
    public abstract partial class SymphonyVisualElement : VisualElement
    {
        /// <summary>
        /// �����������̃^�X�N
        /// </summary>
        public Task InitializeTask { get; private set; }

        public SymphonyVisualElement(string path, InitializeType initializeType = InitializeType.All, LoadType loadType = LoadType.Resources)
        {
            InitializeTask = Initialize(path, initializeType, loadType);
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="path">UXML�̃p�X</param>
        /// <param name="type">�������̃^�C�v</param>
        /// <returns></returns>
        private async Task Initialize(string path, InitializeType type, LoadType loadType)
        {
            VisualTreeAsset treeAsset = default;
            if (!string.IsNullOrEmpty(path))
            {
                switch (loadType)
                {
                    case LoadType.Resources:
                        treeAsset = Resources.Load<VisualTreeAsset>(path);
                        break;

                    case LoadType.Addressable:
                        Debug.LogWarning("Addressable�͌��ݎg�p�ł��܂���");
                        break;

                    case LoadType.AssetDataBase:
#if UNITY_EDITOR
                        treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
#else
                        Debug.Log("AssetDataBase���g�p�������[�h�̓G�f�B�^��p�ł�");
#endif
                        break;
                }
            }
            else
            {
                Debug.LogError($"{name} failed initialize");
                return;
            }


            if (treeAsset != null)
            {
                #region �e�G�������g�̏�����

                var container = treeAsset.Instantiate();
                container.style.width = Length.Percent(100);
                container.style.height = Length.Percent(100);

                if ((type & InitializeType.PickModeIgnore) != 0)
                {
                    this.RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
                    pickingMode = PickingMode.Ignore;

                    container.RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
                    container.pickingMode = PickingMode.Ignore;
                }

                if ((type & InitializeType.Absolute) != 0)
                {
                    this.style.position = Position.Absolute;
                }

                if ((type & InitializeType.FullRangth) != 0)
                {
                    this.style.height = Length.Percent(100);
                    this.style.width = Length.Percent(100);
                }


                hierarchy.Add(container);

                #endregion

                // UI�v�f�̎擾
                await Initialize_S(container);
            }
            else
            {
                Debug.LogError($"Failed to load UXML file \nfrom : {path}");
            }
        }

        /// <summary>
        /// �T�u�N���X�ŗL�̏���������
        /// </summary>
        /// <param name="container">���[�h����UXML�̃R���e�i</param>
        /// <returns></returns>
        protected abstract Task Initialize_S(TemplateContainer container);

        /// <summary>
        /// �������̃^�C�v
        /// </summary>
        [Flags]
        public enum InitializeType
        {
            None = 0,
            Absolute = 1 << 0,
            FullRangth = 1 << 1,
            PickModeIgnore = 1 << 2,
            All = Absolute | FullRangth | PickModeIgnore
        }

        public enum LoadType
        {
            Resources = 0,
            Addressable = 1,
            AssetDataBase = 2,
        }
    }
}
