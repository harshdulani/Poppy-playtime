using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CGTespy.UI;
using TMPro;

public class InGamePanel : MonoBehaviour
{
    public static InGamePanel instance;

    public GameObject levelNum;
    public TextMeshProUGUI levelNumTxt;
    GameEssentials gameEssentials => GameEssentials.instance;
	GAScript ga { get { return GAScript.Instance; } }

    private void Awake()
    {
        instance = this;
        if(gameEssentials)
            levelNumTxt.text = "LEVEL " + gameEssentials.sd.GetLevelNumber();
    }

    void Start()
    {
        if(ga)
            ga.LevelStart(PlayerPrefs.GetInt("levelNo", 1) +" bonus");
    }

    public void TurnOnDefaultIcons()
    {
        levelNum.gameObject.SetActive(true);
    }

    public void DeactiveGO(bool v)
    {
        gameObject.SetActive(v);
    }
    public void Restart()
    {
        GameEssentials.instance.sl.LoadSameScene();
    }

}
