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
        //if(GAScript.Instance)
//            GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("levelNo") + " bonus");

		if (YcHelper.InstanceExists) 
			YcHelper.LevelEnd(true);

        //levelNumber++;
     //   gameEssentials.sd.SetLevelNumber(levelNumber);
    }

    void SaveLevels()
    {
		/*
		 if (ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		*/
		
		if (!YcHelper.InstanceExists || !YcHelper.IsAdAvailable())
		{
			SaveLevelsBehaviour();
			return;
		}
		
		YcHelper.ShowInterstitial(SaveLevelsBehaviour);
	}

	private static void SaveLevelsBehaviour() => SceneChangeManager.LoadNextScene(true);

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
