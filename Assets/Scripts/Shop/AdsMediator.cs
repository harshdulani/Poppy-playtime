using System;
using System.Collections.Generic;

public interface IWantsAds
{
	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo);
	
	public void OnShowDummyAd();
	
	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo);

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo);

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo);
}

public static class AdsMediator
{
	/// <summary>
	/// Used as a replacement action event
	/// </summary>
	public static event Action ShowDummyRewardedAds;
	public static void InvokeShowDummyRewardedAds() => ShowDummyRewardedAds?.Invoke();

	private static readonly List<IWantsAds> Subscribers;

	static AdsMediator()
	{
		//this is initialised the first time someone calls its static methods throughout the game
		Subscribers = new List<IWantsAds>();
	}

	public static void StartListeningForAds(IWantsAds subscriber)
	{
		//check for redundancy
		if(Subscribers.Contains(subscriber)) return;

		Subscribers.Add(subscriber);

		if (!ApplovinManager.instance.enableAds)
		{
			// Subscribe to dummy ad event
			ShowDummyRewardedAds += subscriber.OnShowDummyAd;
			return;
		}
		
		// Subscribe to ad based events
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += subscriber.OnAdRewardReceived;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += subscriber.OnAdFailedToLoad;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += subscriber.OnAdHidden;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += subscriber.OnAdFailed;
	}

	public static void StopListeningForAds(IWantsAds subscriber)
	{
		//only unsubscribe if already subscribed
		if(!Subscribers.Contains(subscriber)) return;
		
		Subscribers.Remove(subscriber);
		
		if (!ApplovinManager.instance.enableAds)
		{
			// Unsubscribe to dummy ad event
			ShowDummyRewardedAds -= subscriber.OnShowDummyAd;
			return;
		}
		
		// Unsubscribe from ad based events
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= subscriber.OnAdRewardReceived;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= subscriber.OnAdFailedToLoad;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= subscriber.OnAdHidden;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= subscriber.OnAdFailed;
	}
}