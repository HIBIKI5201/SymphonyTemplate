using System;
using UnityEngine;

namespace SymphonyFrameWork.CoreSystem
{
    /// <summary>
    /// セーブデータを管理するクラス
    /// </summary>
    /// <typeparam name="DataType">データの型</typeparam>
    public static class SaveDataSystem<DataType> where DataType : new()
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _saveData = null;
        }

        private static SaveData _saveData;
        public static DataType Data
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData.MainData;
            }
        }
        public static string SaveDate
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData.SaveDate;
            }
        }

        public static void Save()
        {
            _saveData = new SaveData(Data);
            string data = JsonUtility.ToJson(_saveData);
            Debug.Log($"{_saveData.SaveDate}\n{data}");
            PlayerPrefs.SetString(typeof(DataType).Name, data);

        }

        private static void Load()
        {
            #region Prefsからデータをロードする
            string json = PlayerPrefs.GetString(typeof(DataType).Name);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"{typeof(DataType).Name}のデータが見つかりませんでした");
                _saveData = new SaveData(new DataType());
                return;
            }
            #endregion

            #region JSONに変換して保存
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data is not null)
            {
                Debug.Log($"{typeof(DataType).Name}のデータがロードされました\n{data}");
                _saveData = data;
            }
            else
            {
                Debug.LogWarning($"{typeof(DataType).Name}のロードが出来ませんでした");
                _saveData = new SaveData(new DataType());
            }
            #endregion
        }

        [Serializable]
        private class SaveData
        {
            public string SaveDate;
            public DataType MainData;

            public SaveData(DataType dataType)
            {
                SaveDate = DateTime.Now.ToString("O");
                MainData = dataType;
            }
        }
    }
}