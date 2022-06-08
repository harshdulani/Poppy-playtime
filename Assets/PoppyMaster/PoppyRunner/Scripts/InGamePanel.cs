using UnityEngine;
using TMPro;

public class InGamePanel : MonoBehaviour
{
    public static InGamePanel instance;

    public GameObject levelNum;
    public TextMeshProUGUI levelNumTxt;
    GameEssentials gameEssentials => GameEssentials.instance;
	//GAScript ga { get { return GAScript.Instance; } }

    private void Awake()
    {
        instance = this;
        if(gameEssentials)
            levelNumTxt.text = "BONUS LEVEL";
    }

    void Start()
    {
        //if(ga)
//            ga.LevelStart(PlayerPrefs.GetInt("levelNo", 1) +" bonus");

		if(YcHelper.InstanceExists)
			YcHelper.LevelStart(PlayerPrefs.GetInt("levelNo", 1) + 1000);
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
