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

        int x = PlayerPrefs.GetInt("lastBuildIndex", 1);

        UnityEngine.SceneManagement.SceneManager.LoadScene(x);
    }
}
