using System;

namespace SymphonyFrameWork.Attribute
{
    /// <summary>
    /// メソッドを実行するボタンを出す
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ContextMenuButtonAttribute : System.Attribute
    {
        public string Text { get; private set; }

        public ContextMenuButtonAttribute(string text)
        {
            Text = text;
        }
    }
}
