using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LevelComplete : MonoBehaviour
{
    public static LevelComplete instace;
    GameEssentials gameEssentials { get { return GameEssentials.instance; } }

    int levelNumber;
    private void Awake()
    {
        instace = this;
       
    }

    void Start()
    {
        levelNumber = gameEssentials.sd.GetLevelNumber();
        if(GAScript.Instance)
            GAScript.Instance.LevelCompleted(levelNumber.ToString());
        //levelNumber++;
     //   gameEssentials.sd.SetLevelNumber(levelNumber);
    }

    void SaveLevels()
    {
        int currentSceneIndex = gameEssentials.sd.GetLevelNumber();
        int totalBuildSettingsScenes = gameEssentials.sl.GetTotalScenesInBuildSettings();

        gameEssentials.sd.SetLevelNumber(currentSceneIndex + 1);
        gameEssentials.sd.SaveData();

        if (currentSceneIndex >= totalBuildSettingsScenes - 1)
        {
            gameEssentials.sl.LoadSceneByInt(UnityEngine.Random.Range(2, totalBuildSettingsScenes - 1));
        }
        else
        {
            gameEssentials.sl.LoadSceneByInt(gameEssentials.sl.GetNextSceneByBuildIndex());
        }
    }
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
