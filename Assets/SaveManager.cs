using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Number of save slots
    public static int NumSaveSlots = 3;

    // Example data structure, replace it with your own game data structure
    [System.Serializable]
    public class GameData
    {
        public float mapLatitude;
        public float mapLongitude;
        public float mapZoomLevel;
    }

    public static void SaveGame(int slotIndex, GameData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("GameData_" + slotIndex, jsonData);
        PlayerPrefs.Save();
    }

    public static GameData LoadGame(int slotIndex)
    {
        string key = "GameData_" + slotIndex;
        if (PlayerPrefs.HasKey(key))
        {
            string jsonData = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<GameData>(jsonData);
        }
        else
        {
            // Return default data if no saved data is found
            return new GameData();
        }
    }
}
