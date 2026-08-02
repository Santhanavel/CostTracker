using System.IO;
using UnityEngine;
using FoodTracker.Data;

namespace FoodTracker.Persistence
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public AppData AppData { get; private set; }

        private string savePath;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                savePath = Path.Combine(Application.persistentDataPath, "AppData.json");
                Load();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(AppData, true);
                File.WriteAllText(savePath, json);
                Debug.Log($"App data saved to: {savePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save app data: {ex.Message}");
            }
        }

        public void Load()
        {
            if (File.Exists(savePath))
            {
                try
                {
                    string json = File.ReadAllText(savePath);
                    AppData = JsonUtility.FromJson<AppData>(json);
                    Debug.Log("App data loaded successfully.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load app data: {ex.Message}. Creating default data.");
                    CreateDefaultData();
                }
            }
            else
            {
                CreateDefaultData();
            }
        }

        private void CreateDefaultData()
        {
            AppData = new AppData();
            Save();
            Debug.Log("New default AppData created and saved.");
        }

        public void ClearData()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            CreateDefaultData();
        }
    }
}
