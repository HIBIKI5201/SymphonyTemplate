

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class VersionLogGenerator : EditorWindow
{
    private const string logPath = "Assets/SymphonyFrameWork/CHANGELOG.md";

    private static Action OnShowWindow;

    private static List<LogData> logs = new();

    private LogData data;

    VersionLogGenerator()
    {
        OnShowWindow += () =>
        {
            data.version = logs.FirstOrDefault().version;
        };
    }

    [MenuItem("Tools/" + nameof(VersionLogGenerator))]
    public static void ShowWindow()
    {
        // ウィンドウを表示
        var window = GetWindow<VersionLogGenerator>();
        window.titleContent = new GUIContent(nameof(VersionLogGenerator));
        window.Show();

        var log = ReadChangelog();
        ConvertLogData(log);

        OnShowWindow?.Invoke();
    }

    private static string ReadChangelog()
    {
        if (File.Exists(logPath))
        {
            return File.ReadAllText(logPath);
        }
        else
        {
            Debug.LogError("CHANGELOG.md が見つかりません");
            return string.Empty;
        }
    }

    private static void ConvertLogData(string text)
    {
        var lines = text.Split("\n").Select(l => l.TrimEnd('\r')).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("## "))
            {
                var data = new LogData();

                data.version = lines[i].Substring(lines[i].IndexOf("[") + 1, lines[i].IndexOf("]") - lines[i].IndexOf("[") - 1);
                data.date = lines[i].Substring(lines[i].IndexOf("- ") + 2);

                data.type = StringToLogType(lines[i + 1].Substring(4));

                int counter = 0;
                while (!string.IsNullOrEmpty(lines[i + 2 + counter]) && lines[i+ 2 + counter][0] == '-')
                {
                    data.text = lines[i + 2 + counter];
                    counter++;
                }

                logs.Add(data);
            }
        }
    }

    // ウィンドウのGUIを描画
    private void OnGUI()
    {
        DateTime date = DateTime.Now;
        data.date = $"{date.Year.ToString("0000")}-{date.Month.ToString("00")}-{date.Day.ToString("00")}";
        GUILayout.Label("date: " + data.date);

        if (GUILayout.Button("Add Log"))
        {
            AddLog();
        }

        data.type = (LogType)EditorGUILayout.EnumPopup("Player Class", data.type);

        // テキストフィールドを追加
        var text = EditorGUILayout.TextField("version", data.version);

        // フィールドの値を表示
        GUILayout.Label("Window Title: " + text);
    }
    
    private void AddLog()
    {
        if (logs.Select(data => data.version).Contains(data.version))
        {
            Debug.LogWarning($"version :{data.version}は既に存在します");
            return;
        }


    }

    [Serializable]
    private struct LogData
    {
        public string version;
        public string date;
        public LogType type;
        public string text;

        public override string ToString() =>
            $"## [{version}]  - {date}\n" +
            LogTypeToString(type) + $"\n- {text}";


    }

    private static LogType StringToLogType(string str) => str switch
    {
        "Add" => LogType.Add,
        "Update" => LogType.Update,
        "Fix" => LogType.Fix,
        _ => LogType.Other
    };

    private static string LogTypeToString(LogType type) => type switch
    {
        LogType.Add => "Add",
        LogType.Update => "Update",
        LogType.Fix => "Fix",
        _ => string.Empty
    };


    private enum LogType
    {
        Add,
        Update,
        Fix,
        Other,
    }
}