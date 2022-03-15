using UnityEngine;

public class SavedData : MonoBehaviour
{
    public static SavedData instance;

    public const string savedScene = "SavedScene";
    public const string levelNumber = "LevelNumber";

    public const string audioState = "SoundState";
    public const string hepaticState = "HapticState";

    public const string totalCoins = "TotalCoins";


    private void Awake()
    {
        instance = this;
    }


    // Get Value // Get//

    public int GetLevelNumber()
    {
        return PlayerPrefs.GetInt(levelNumber, 1);
    }

    public string GetSavedScene()
    {
        return PlayerPrefs.GetString(savedScene, "1");
    }

    public int GetSavedSceneByInt()
    {
        return PlayerPrefs.GetInt(savedScene, 1);
    }

    public string GetSoundState()
    {
        return PlayerPrefs.GetString(audioState, "On");
    }

    public string GetHepaticState()
    {
        return PlayerPrefs.GetString(hepaticState, "On");
    }

    public int GetTotalCoins()
    {
        return PlayerPrefs.GetInt(totalCoins, 0);
    }

    // Set Value // Set//

    public void SetSavedScene(string val)
    {
        PlayerPrefs.SetString(savedScene, val);
    }
    public void SetSavedSceneByInt(int val)
    {
        PlayerPrefs.SetInt(savedScene, val);
    }

    public void SetLevelNumber(int val)
    {
        PlayerPrefs.SetInt(levelNumber, val);
    }

    public void SetSoundState(string state)
    {
        PlayerPrefs.SetString(audioState, state);
    }

    public void SetHepaticState(string state)
    {
        PlayerPrefs.SetString(hepaticState, state);
    }

    public void SetTotalCoins(int coins)
    {
        PlayerPrefs.SetInt(totalCoins, coins);
    }


    public void SaveData()
    {
        PlayerPrefs.Save();
    }
}
