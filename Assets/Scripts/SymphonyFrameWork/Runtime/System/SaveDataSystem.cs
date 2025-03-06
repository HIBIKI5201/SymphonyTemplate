using Newtonsoft.Json;
using System;
using UnityEngine;

namespace SymphonyFrameWork.System
{
    /// <summary>
    ///     セーブデータを管理するクラス
    /// </summary>
    /// <typeparam name="DataType">データの型</typeparam>
    public static class SaveDataSystem<DataType> where DataType : class, new()
    {
        private static SaveData _saveData;

        public static DataType Data
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData?.MainData;
            }
        }

        public static string SaveDate
        {
            get
            {
                if (_saveData is null)
                    Load();
                return _saveData?.SaveDate;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _saveData = null;
        }

        public static void Save()
        {
            _saveData = new SaveData(Data);
            var data = JsonConvert.SerializeObject(_saveData);
            Debug.Log($"{_saveData.SaveDate}\n{data}");
            PlayerPrefs.SetString(typeof(DataType).Name, data);
        }

        private static void Load()
        {
            #region Prefsからデータをロードする

            var json = PlayerPrefs.GetString(typeof(DataType).Name);
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log($"{typeof(DataType).Name}のデータが見つからないので生成しました");
                _saveData = new SaveData(new DataType());
                return;
            }

            #endregion

            #region JSONに変換して保存

            var data = JsonConvert.DeserializeObject<SaveData>(json);
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