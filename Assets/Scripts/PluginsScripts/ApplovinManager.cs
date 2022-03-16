using DG.Tweening;
using UnityEngine;

public class ApplovinManager : MonoBehaviour
{
	public static ApplovinManager instance;

	public bool enableAds;
	
	// Retrieve the ID's from your account
    public string bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";
    public string rewardedAdUnitId = "YOUR_REWARDED_AD_UNIT_ID";
    public string interstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID";
    
    int retryAttemptInter, retryAttemptRewarded;

    void Awake()
    {
        instance = this;

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads

            if (bannerAdUnitId == "" || rewardedAdUnitId == "" || interstitialAdUnitId == "")
                return;

            print("contains ids");

            InitializeBannerAds();

            InitializeRewardedAds();

            InitializeInterstitialAds();

            ShowBannerAds();

        };

        MaxSdk.SetSdkKey("qx1n7X8i53FgIANTP6L6vRD5KlRgJCW87XvF41y94CrNMDsnJBuDd6Jyrlc9x8H3fAJlCGuHSkfqxVaFgTSIZn");
        MaxSdk.InitializeSdk();
    }

    public bool RewardAdsAvailable()
    {
        return MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
    }

    public bool InterstitialAdsAvailable()
    {
        return MaxSdk.IsInterstitialReady(interstitialAdUnitId);
    }

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320�50 on phones and 728�90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
    }

    public void ShowBannerAds()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void HideBannerAds()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
    }

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

       // Load the first interstitial
       LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttemptInter = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttemptInter++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptInter));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
       
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
    {
        
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }

    public void ShowInterstitialAds()
    {
        if (interstitialAdUnitId == "")
            return;

        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }
    }

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
		
        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptRewarded++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptRewarded));

        Invoke(nameof(LoadRewardedAd), (float)retryDelay);
	}

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
	}

	private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		
	}

	private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		// Rewarded ad is hidden. Pre-load the next ad
		LoadRewardedAd();
	}

	private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
		// The rewarded ad displayed and the user should receive the reward.
	}
	
    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
		print("sss");
        // Ad revenue paid. Use this callback to track user revenue.
    }

    public bool TryShowRewardedAds()
    {
		if (!enableAds)
		{
			DOVirtual.DelayedCall(0.5f, AdsMediator.InvokeShowDummyRewardedAds);
			return true;
		}
		
		if(rewardedAdUnitId == "") return false;
		if (!MaxSdk.IsRewardedAdReady(rewardedAdUnitId)) return false;
		
		MaxSdk.ShowRewardedAd(rewardedAdUnitId);
		return true;
	}
}