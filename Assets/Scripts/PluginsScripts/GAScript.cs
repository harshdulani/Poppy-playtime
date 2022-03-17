
using System.Collections;
using UnityEngine;
using GameAnalyticsSDK;

public class GAScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static GAScript Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        GameAnalytics.Initialize();
	}

    private void Start()
    {
        GameObject.Find("EventSystem").SetActive(false);
		StartCoroutine(SetControlMechanic());
	}

	private IEnumerator SetControlMechanic()
	{
		yield return new WaitUntil(GameAnalytics.IsRemoteConfigsReady);
		
		PlayerPrefs.SetInt("controlMechanic", int.Parse(GameAnalytics.GetRemoteConfigsValueAsString("Input", "0")));
	}

    public void LevelStart(string levelname)
    {
		GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelname);
		//FlurryStart.instance?.LevelStart(levelname);
        //FaceBookScript.instance.LevelStarted(levelname);
       
    }

    public void LevelFail(string levelname)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelname);
		FlurryStart.instance?.LevelFail(levelname);
       // FaceBookScript.instance.LevelFailed(levelname);
       
    }

    public void LevelCompleted(string levelname)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelname);
		//FlurryStart.instance?.LevelCompleted(levelname);
		// FaceBookScript.instance.LevelCompleted(levelname);

	}
}