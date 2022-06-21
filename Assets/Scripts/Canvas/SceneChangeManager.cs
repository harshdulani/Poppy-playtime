using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneChangeManager
{
	public static void LoadNextScene(bool increaseBonusLevelAlso = false)
	{
		var lastRegularLevel = PlayerPrefs.GetInt("lastRegularLevel");

		JustIncreaseCurrentLevelNo();

		if (increaseBonusLevelAlso) JustIncreaseBonusLevelEncounters();

		if (PlayerPrefs.GetInt("levelNo", 1) - PlayerPrefs.GetInt("encounteredBonusLevels", 0) <= lastRegularLevel)
		{
			//we want this to be minus the encounteredBonusLevels because we want to get out of it a build index over here
			var x = PlayerPrefs.GetInt("levelNo", 1) - PlayerPrefs.GetInt("encounteredBonusLevels");
			Debug.Log($"load level no {x} = ({PlayerPrefs.GetInt("levelNo", 1)} - enc {PlayerPrefs.GetInt("encounteredBonusLevels")})");
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, lastRegularLevel + 1);
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		
		AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);
	}

	//this number "levelNo" can be thought of as no of levels encountered/ levels passed + 1 (to show current level)
	public static void JustIncreaseCurrentLevelNo()
	{
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
		Debug.Log($"increase levelNo to {PlayerPrefs.GetInt("levelNo", 1)}, without loading scene");
	}

	public static void JustIncreaseBonusLevelEncounters()
	{
		PlayerPrefs.SetInt("encounteredBonusLevels", PlayerPrefs.GetInt("encounteredBonusLevels", 0) + 1);
		Debug.Log($"increase enc bonus levels to {PlayerPrefs.GetInt("encounteredBonusLevels", 0)}");
	}
}