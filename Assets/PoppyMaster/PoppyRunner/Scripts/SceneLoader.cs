using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetTotalScenesInBuildSettings()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    public int GetCurrentSceneByBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public string GetCurrentSceneByName()
    {
        return SceneManager.GetActiveScene().name;
    }
    public string GetNextSceneByName()
    {
        return (SceneManager.GetActiveScene().buildIndex + 1).ToString();
    }

    public int GetNextSceneByBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    } 
    
    public void LoadSceneByInt(int SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void LoadSameScene()
    {
        SceneManager.LoadScene(GetCurrentSceneByName());
    }
}
