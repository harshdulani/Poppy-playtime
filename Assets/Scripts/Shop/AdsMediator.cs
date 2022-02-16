using System.Collections.Generic;
using UnityEngine;

public interface IWantsAds
{
	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo);
	
	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo);

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo);

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo);
}

public static class AdsMediator
{
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
		// Unsubscribe from ad based events
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= subscriber.OnAdRewardReceived;
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= subscriber.OnAdFailedToLoad;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= subscriber.OnAdHidden;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= subscriber.OnAdFailed;
	}
}