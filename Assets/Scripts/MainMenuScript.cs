using System.Collections;
using TMPro;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
	public TextMeshProUGUI text;
	
    private void Start()
    {
        if (!PlayerPrefs.HasKey("lastBuildIndex"))
        {
            PlayerPrefs.SetInt("lastBuildIndex", 1);
            PlayerPrefs.SetInt("levelNo", 1);
        }

        int x = PlayerPrefs.GetInt("lastBuildIndex", 1);
		text.text = x.ToString();
		UnityEngine.SceneManagement.SceneManager.LoadScene(x);
	}

}
