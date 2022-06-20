using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("lastBuildIndex"))
        {
            PlayerPrefs.SetInt("lastBuildIndex", 1);
            PlayerPrefs.SetInt("levelNo", 1);
            PlayerPrefs.SetInt("encounteredBonusLevels", 0);
        }

		UnityEngine.SceneManagement.SceneManager.LoadScene(PlayerPrefs.GetInt("lastBuildIndex", 1));
	}
}
