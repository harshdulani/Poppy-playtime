using System;
using UnityEngine;

public class YcHelper : MonoBehaviour
{
	public static bool InstanceExists => YsoCorp.GameUtils.YCManager.instance;
	public static bool IsAdAvailable() => YsoCorp.GameUtils.YCManager.instance.adsManager.IsRewardBasedVideo();
	

	public static void LevelStart(int levelNum) => YsoCorp.GameUtils.YCManager.instance.OnGameStarted(levelNum);

	public static void LevelEnd(bool hasWonLevel) => YsoCorp.GameUtils.YCManager.instance.OnGameFinished(hasWonLevel);
	
	public static void ShowGDPR() => YsoCorp.GameUtils.YCManager.instance.settingManager.Show(); 

	public static void ShowInterstitial(Action functionCall)
	{
		// TODO call the action (eg: play, restart, back, next level, ...)
		YsoCorp.GameUtils.YCManager.instance.adsManager.ShowInterstitial(functionCall);
	}

	public static void ShowRewardedAds(Action functionCall)
	{
		//if everything went ok while trying to show ad, then show it otherwise don't
		YsoCorp.GameUtils.YCManager.instance.adsManager.ShowRewarded((bool ok) =>
		{
			if (ok)
				// TODO give the reward to the user
				functionCall();
		});
	}
}