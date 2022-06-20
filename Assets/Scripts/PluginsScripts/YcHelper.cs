using System;
using YsoCorp.GameUtils;

public static class YcHelper
{
	public static bool InstanceExists => YCManager.instance;
	public static bool IsAdAvailable() => YCManager.instance.adsManager.IsRewardBasedVideo();

	public static void LevelStart(int levelNum) => YCManager.instance.OnGameStarted(levelNum);

	public static void LevelEnd(bool hasWonLevel) => YCManager.instance.OnGameFinished(hasWonLevel);
	
	public static void ShowGDPR() => YCManager.instance.settingManager.Show();

	public static bool GetIsComboTextTestingOn() => YCManager.instance.abTestingManager.IsPlayerSample("comboText");
	public static bool GetIsObjectUnlockTestingOn() => YCManager.instance.abTestingManager.IsPlayerSample("objectUnlock");

	public static void ShowInterstitial(Action functionCall)
	{
		// TODO call the action (eg: play, restart, back, next level, ...)
		YCManager.instance.adsManager.ShowInterstitial(functionCall);
	}

	public static void ShowRewardedAds(Action functionCall)
	{
		//if everything went ok while trying to show ad, then show it otherwise don't
		YCManager.instance.adsManager.ShowRewarded((bool ok) =>
		{
			if (ok)
				// TODO give the reward to the user
				functionCall();
		});
	}
}