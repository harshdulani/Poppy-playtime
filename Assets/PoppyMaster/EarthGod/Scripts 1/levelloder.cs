using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelloder : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("level")>= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("THISLEVEL"));
        }
        else
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("level",1));
        }
    }
}
