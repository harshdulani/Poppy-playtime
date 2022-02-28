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
    GameEssentials gameEssentials { get { return GameEssentials.instance; } }
    GAScript ga { get { return GAScript.Instance; } }

    private void Awake()
    {
        instance = this;
        if(gameEssentials)
            levelNumTxt.text = "LEVEL " + gameEssentials.sd.GetLevelNumber().ToString();
    }

    void Start()
    {
        if(ga)
            ga.LevelStart(gameEssentials.sd.GetLevelNumber()+"");
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
