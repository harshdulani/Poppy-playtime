using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    private void Start()
    {
        if (!PlayerPrefs.HasKey("lastBuildIndex"))
        {
            PlayerPrefs.SetInt("lastBuildIndex", 1);
            PlayerPrefs.SetInt("levelNo", 1);
        }

		PlayerPrefs.SetInt("controlMechanic", 1);

		UnityEngine.SceneManagement.SceneManager.LoadScene(PlayerPrefs.GetInt("lastBuildIndex", 1));
	}
}
