public interface IWantsAds
{
	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo);
	
	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo);

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo);

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo);
}

public static class AdsMediator
{
	public static void StartListeningForAds(IWantsAds subscriber)
	{
		// Subscribe to ad based events
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += subscriber.OnAdRewardReceived;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += subscriber.OnAdFailedToLoad;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += subscriber.OnAdHidden;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += subscriber.OnAdFailed;
	}
	
	public static void StopListeningForAds(IWantsAds subscriber)
	{
		// Unsubscribe from ad based events
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= subscriber.OnAdRewardReceived;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= subscriber.OnAdFailedToLoad;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= subscriber.OnAdHidden;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= subscriber.OnAdFailed;
	}
}