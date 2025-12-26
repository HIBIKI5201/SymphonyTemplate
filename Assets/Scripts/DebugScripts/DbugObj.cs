using SymphonyFrameWork;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.Debugger;
using TestNameSpace;
using UnityEngine;

public class DbugObj : MonoBehaviour, IGameObject
{
    [SerializeField] private float _speed = 5;

    [DisplayText("試し書き試し書き試し書き")]
    [ReadOnly]
    [SerializeField]
    private Vector3 _velocity;
    [SerializeField, TagSelector]
    private string _tag;

    [SerializeField] private AnimationCurve _curve;

    private SymphonyDebugHUD _debugHUD;

    [SerializeField]
    private Color _color;

    [SerializeReference, SubclassSelector]
    private ITestInterface _g;

    private void Update()
    {
        SymphonyDebugLogger.NewText("test_text".AddRichTextColor(_color).RemoveRichTextColor());
        SymphonyDebugLogger.AddText("text2".AddRichTextBold().RemoveRichTextBold());
        SymphonyDebugLogger.AddText("text3".AddRichTextUnderline().RemoveRichTextUnderline());
        SymphonyDebugLogger.LogText(text:"text4".AddRichTextBold().AddRichTextUnderline().AddRichTextBold().RemoveRichTextUnderline().RemoveRichTextBold());
    }
}