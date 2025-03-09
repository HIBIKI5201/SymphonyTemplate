

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

    private LogData data = new();

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
        logs.Clear();

        var lines = text.Split("\n").Select(l => l.TrimEnd('\r')).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("## "))
            {
                var data = new LogData();

                data.version = lines[i].Substring(lines[i].IndexOf("[") + 1, lines[i].IndexOf("]") - lines[i].IndexOf("[") - 1);
                data.date = lines[i].Substring(lines[i].IndexOf("- ") + 2);

                string title = string.Empty;
                i += 1;
                while (!string.IsNullOrEmpty(lines[i]))
                {
                    if (lines[i].StartsWith("### "))
                    {
                        title = lines[i].Substring(4);
                    }
                    else if (lines[i].StartsWith("- "))
                    {
                        switch (title)
                        {
                            case "Add":
                                data.addText.Add(lines[i].Substring(2));
                                break;
                            case "Update":
                                data.updateText.Add(lines[i].Substring(2));
                                break;
                            case "Fix":
                                data.fixText.Add(lines[i].Substring(2));
                                break;
                        }
                    }
                    i++;
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

        // テキストフィールドを追加
        data.version = EditorGUILayout.TextField("version", data.version);

        // リストの編集
        ListGUI("AddText", ref data.addText);
        ListGUI("UpdateText", ref data.updateText);
        ListGUI("FixText", ref data.fixText);
    }

    private void ListGUI(string label, ref List<string> list)
    {
        EditorGUILayout.Space(10);
        GUILayout.Label(label);

        for (int i = 0; i < list.Count; i++)
        {
            // 各要素の編集フィールド
            list[i] = EditorGUILayout.TextField("Item " + i, list[i]);

            // 要素を削除するボタン
            if (GUILayout.Button("Remove Item " + i))
            {
                list.RemoveAt(i);
            }
        }

        // 新しいアイテムを追加するボタン
        if (GUILayout.Button("Add Item"))
        {
            list.Add(string.Empty);
        }
    }

    private void AddLog()
    {
        if (logs.Select(data => data.version).Contains(data.version))
        {
            Debug.LogWarning($"version :{data.version}は既に存在します");
            return;
        }

        logs = new List<LogData>() { data }.Concat(logs).ToList();

        string text = $"# Changelog\n\n" +
            string.Join("\n\n", logs);

        File.WriteAllText(logPath, text);
    }

    [Serializable]
    private class LogData
    {
        public string version;
        public string date;
        public List<string> addText;
        public List<string> updateText;
        public List<string> fixText;

        public LogData()
        {
            version = string.Empty;
            date = "2000-01-01";
            addText = new();
            updateText = new();
            fixText = new();
        }

        public override string ToString() =>
            $"## [{version}] - {date}" +
            (addText.Count > 0 ? "\n### Add" + $"\n- {string.Join("\n- ", addText)}" : string.Empty)+
            (updateText.Count > 0 ? "\n### Update" + $"\n- {string.Join("\n- ", updateText)}" : string.Empty) +
            (fixText.Count > 0 ? "\n### Fix" + $"\n- {string.Join("\n- ", fixText)}" : string.Empty);
    }

    private enum LogType
    {
        Add,
        Update,
        Fix,
        Other,
    }
}