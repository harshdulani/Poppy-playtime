
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ui : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject LossPanel;
    public static Ui instance;
    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }
    public void Start()
    {
        winPanel.SetActive(false);
        LossPanel.SetActive(false);
     
        //if(AudioManager.instance)
        //{
        //    AudioManager.instance.Pause("GroundCrack");
        //}
    }
    public void next()
    {
        if (PlayerPrefs.GetInt("level") >= (SceneManager.sceneCountInBuildSettings)-1)
        {
            PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level",1) + 1);
            int i = Random.Range(1, (SceneManager.sceneCountInBuildSettings));
            PlayerPrefs.SetInt("THISLEVEL", i);
            SceneManager.LoadScene(i);
        }
        else
        {
            PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void loop()
    {
        SceneManager.LoadScene(1);
    }
}
