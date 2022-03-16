using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public static LevelComplete instace;
    GameEssentials gameEssentials { get { return GameEssentials.instance; } }

    private int _lastRegularLevel;
    private void Awake()
    {
        instace = this;
	}

    void Start()
    {
		_lastRegularLevel = PlayerPrefs.GetInt("lastRegularLevel");
        if(GAScript.Instance)
            GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("levelNo") + " bonus");
        //levelNumber++;
     //   gameEssentials.sd.SetLevelNumber(levelNumber);
    }

    void SaveLevels()
    {
		if (ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		
		if (PlayerPrefs.GetInt("levelNo", 1) < _lastRegularLevel)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, _lastRegularLevel);
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}

		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
	}
	
    public void Next()
    {
        // gameEssentials.sl.LoadSceneByInt(gameEssentials.sd.GetLevelNumber());
        SaveLevels();
       // gameEssentials.sl.LoadSceneByInt(gameEssentials.sl.GetCurrentSceneByBuildIndex()+1);
    }

    public void DeactiveGO(bool v)
    {
        gameObject.SetActive(v);
    }
}
