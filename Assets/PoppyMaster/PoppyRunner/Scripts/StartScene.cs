using UnityEngine;

public class StartScene : MonoBehaviour
{
    public bool isTesting;
    public int levelName;

    GameEssentials gameEssentials { get { return GameEssentials.instance; } }

    void Start()
    {
        if (isTesting)
        {
            gameEssentials.sl.LoadSceneByInt(levelName);
        }
        else
        {
            int currentSceneIndex = gameEssentials.sd.GetLevelNumber();
            int totalBuildSettingsScenes = gameEssentials.sl.GetTotalScenesInBuildSettings();

            if (currentSceneIndex >= totalBuildSettingsScenes - 1)
            {
                gameEssentials.sl.LoadSceneByInt(Random.Range(1, totalBuildSettingsScenes - 1));
            }
            else
            {
                gameEssentials.sl.LoadSceneByInt(currentSceneIndex);
            }
        }

    }
}
