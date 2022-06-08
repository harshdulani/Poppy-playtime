using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
	Locked, Unlocked, Selected
}

public class ShopItem : MonoBehaviour//, IWantsAds
{
	[SerializeField] private ShopItemState myState;
	[SerializeField] private Image unlocked, selected, icon, unavailable;
	[SerializeField] private TextMeshProUGUI costText, questionMark;
	[SerializeField] private Color cantBuyColor;

	[SerializeField] private Mask iconMask;
	[SerializeField] private bool shouldUseMask;

	[SerializeField] private GameObject stickyCost, stickyAds;

	private static bool _canClickOnItem = true;
	private const float CanTouchCooldownTimer = 0.25f;

	private bool _isWeaponItem, _isAvailable;
	private int _mySkinIndex;

	//private void OnDestroy() => AdsMediator.StopListeningForAds(this);

	public void SetIconSprite(Sprite image)
	{
		iconMask.graphic.enabled = shouldUseMask;
		if (image)
		{
			icon.sprite = image;
			icon.enabled = true;
			questionMark.enabled = false;
		}
		else
		{
			icon.enabled = false;
			questionMark.enabled = true;
		}
	}

	public void SetState(ShopItemState state)
	{
		myState = state;
		switch (myState)
		{
			case ShopItemState.Locked:
				unlocked.enabled = false;
				selected.enabled = false;
				//sticky cost or sticky ads will be turned on according to availability, already in the loop of the calling function
				break;
			case ShopItemState.Unlocked:
				unlocked.enabled = true;
				selected.enabled = false;
				stickyAds.SetActive(false);
				break;
			case ShopItemState.Selected:
				unlocked.enabled = false;
				selected.enabled = true;
				stickyAds.SetActive(false);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		unavailable.gameObject.SetActive(false);
	}

	public void SetSkinIndex(int idx) => _mySkinIndex = idx;

	public void SetIsWeaponItem(bool status) => _isWeaponItem = status;

	public void SetPriceAndAvailability(int price)
	{
		if (myState == ShopItemState.Selected || myState == ShopItemState.Unlocked)
		{
			// if the item is already owned
			costText.gameObject.SetActive(false);
			unavailable.gameObject.SetActive(false);
			stickyCost.SetActive(false);
			return;
		}

		costText.text = price.ToString();
		_isAvailable = CheckAvailability(price);

		stickyAds.SetActive(!_isAvailable && (YcHelper.InstanceExists && YcHelper.IsAdAvailable()));

		costText.color = _isAvailable ? Color.white : cantBuyColor;
		unavailable.gameObject.SetActive(!_isAvailable);
	}

	private static bool CheckAvailability(int price) => price <= ShopStateController.CurrentState.GetState().CoinCount;

	public void ClickOnLocked()
	{
		if (myState != ShopItemState.Locked) return;

		if(!_canClickOnItem) return;

		_canClickOnItem = false;
		DOVirtual.DelayedCall(CanTouchCooldownTimer, () => _canClickOnItem = true);
		
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx

		if (_isAvailable)
		{
			if (_isWeaponItem)
				GameEvents.Only.InvokeWeaponSelect(_mySkinIndex, true);
			else
				GameEvents.Only.InvokeSkinSelect(_mySkinIndex, true);
			return;
		}

		/*
		if (!ApplovinManager.instance) return;
		if (!ApplovinManager.instance.enableAds) return;
		
		if (!ApplovinManager.instance.TryShowRewardedAds()) return;

		AdsMediator.StartListeningForAds(this);
		*/
		
		if (!YcHelper.InstanceExists || !YcHelper.IsAdAvailable()) return;
		
		YcHelper.ShowRewardedAds(AdRewardReceiveBehaviour);
	}

	public void ClickOnUnlocked()
	{
		if(!_canClickOnItem) return;

		_canClickOnItem = false;
		DOVirtual.DelayedCall(CanTouchCooldownTimer, () => _canClickOnItem = true);
		
		if (_isWeaponItem)
			GameEvents.Only.InvokeWeaponSelect(_mySkinIndex, false);
		else
			GameEvents.Only.InvokeSkinSelect(_mySkinIndex, false);

		AudioManager.instance.Play("Button");
	}

	private void AdRewardReceiveBehaviour()
	{
		if (_isWeaponItem)
			GameEvents.Only.InvokeWeaponSelect(_mySkinIndex, false);
		else
			GameEvents.Only.InvokeSkinSelect(_mySkinIndex, false);

		//AdsMediator.StopListeningForAds(this);
	}

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo) => AdRewardReceiveBehaviour();

	/*
	public void OnShowDummyAd() => AdRewardReceiveBehaviour();

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo) => AdsMediator.StopListeningForAds(this);

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) => AdsMediator.StopListeningForAds(this);

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo) => AdsMediator.StopListeningForAds(this);
	*/
}