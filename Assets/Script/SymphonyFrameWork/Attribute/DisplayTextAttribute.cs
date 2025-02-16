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
}
