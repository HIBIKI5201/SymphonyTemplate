using SymphonyFrameWork.CoreSystem;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Debugger")]
public class Debugger : ScriptableObject
{
    public void PauseAndResume()
    {
        PauseManager.Pause = !PauseManager.Pause;
    }


}

#if UNITY_EDITOR

[CustomEditor(typeof(Debugger))]
public class MyScriptEditor : Editor
{
    public override async void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        var storyData = target as Debugger;

        if (GUILayout.Button("ポーズとリズーム"))
        {
            storyData.PauseAndResume();
        }
    }
}
#endif
