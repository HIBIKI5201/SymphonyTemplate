using SymphonyFrameWork;
using SymphonyFrameWork.Attribute;
using SymphonyFrameWork.Debugger;
using SymphonyFrameWork.Debugger.HUD;
using SymphonyFrameWork.System.SaveSystem;
using SymphonyFrameWork.System.ServiceLocate;
using System;
using TestNameSpace;
using UnityEngine;

public class DbugObj : MonoBehaviour, IGameObject, IInjectable<MeshRenderer>
{
    [SerializeField] private float _speed = 5;

    [DisplayText("試し書き試し書き試し書き")]
    [ReadOnly]
    [SerializeField]
    private Vector3 _velocity;
    [SerializeField, TagSelector]
    private string _tag;

    [SerializeField] private AnimationCurve _curve;

    [SerializeField]
    private Color _color;

    [SerializeReference, SubclassSelector]
    private ITestInterface _g;

    public void Start()
    {
        ServiceInjector.Inject(this);

        SymphonyDebugHUD.AddText(() => $"time pp: {Time.time}");

        SaveAndLoadDebug();
    }

    public async void SaveAndLoadDebug()
    {
        var data = await SaveSystem<TestData, NugetDataLoader<TestData>>.Get();
        Debug.Log(data);

        data.Name = "ChangedName";
        Debug.Log(data);

        var data2 = await SaveSystem<TestData, NugetDataLoader<TestData>>.Get();
        Debug.Log(data2);

        await SaveSystem<TestData, NugetDataLoader<TestData>>.Save();
    }

    public void Inject(MeshRenderer meshRenderer)
    {
        Debug.Log(meshRenderer.name);
    }

    private void Update()
    {
        SymphonyDebugLogger.NewText("test_text".AddRichTextColor(_color).RemoveRichTextColor());
        SymphonyDebugLogger.AddText("text2".AddRichTextBold().RemoveRichTextBold());
        SymphonyDebugLogger.AddText("text3".AddRichTextUnderline().RemoveRichTextUnderline());
        SymphonyDebugLogger.LogText(text: "text4".AddRichTextBold().AddRichTextUnderline().AddRichTextBold().RemoveRichTextUnderline().RemoveRichTextBold());
    }

    [Serializable]
    private class TestData
    {
        public string Name { get; set; } = "TestName";

        public override string ToString()
        {
            return $"name is {Name}";
        }
    }
}