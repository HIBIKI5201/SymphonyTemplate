using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SymphonyFrameWork.Utility
{
    [UxmlElement]
    public abstract partial class SymphonyVisualElement : VisualElement
    {
        public Task InitializeTask { get; private set; }

        public SymphonyVisualElement(string path, InitializeType type = InitializeType.All)
        {
            InitializeTask = Initialize(path, type);
        }

        private async Task Initialize(string path, InitializeType type)
        {
            VisualTreeAsset treeAsset = default;
            if (!string.IsNullOrEmpty(path))
            {
                treeAsset = Resources.Load<VisualTreeAsset>(path);
            }
            else
            {
                Debug.LogError($"{name} failed initialize");
                return;
            }


            if (treeAsset != null)
            {
                #region 親エレメントの初期化

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

                // UI要素の取得
                await Initialize_S(container);
            }
            else
            {
                Debug.LogError($"Failed to load UXML file \nfrom : {path}");
            }
        }

        protected abstract Task Initialize_S(TemplateContainer container);

        [Flags]
        public enum InitializeType
        {
            None = 0,
            Absolute = 1 << 0,
            FullRangth = 1 << 1,
            PickModeIgnore = 1 << 2,
            All = Absolute | FullRangth | PickModeIgnore
        }
    }
}
