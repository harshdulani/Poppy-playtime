using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
	Locked, Unlocked, Selected
}

public class ShopItem : MonoBehaviour, IWantsAds
{
	[SerializeField] private ShopItemState myState;
	[SerializeField] private Image unlocked, selected, icon, unavailable;
	[SerializeField] private TextMeshProUGUI costText, questionMark;
	[SerializeField] private Color cantBuyColor;

	[SerializeField] private Mask iconMask;
	[SerializeField] private bool shouldUseMask;

	[SerializeField] private GameObject stickyCost, stickyAds;
	[HideInInspector] public bool shouldWaitForAd;

	private bool _isWeaponItem, _isAvailable;
	private int _mySkinIndex;

	private void OnDisable()
	{
		if (!shouldWaitForAd) return;

		this.StopListeningForAds();
	}

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
				stickyCost.SetActive(false);
				stickyAds.SetActive(false);
				break;
			case ShopItemState.Selected:
				unlocked.enabled = false;
				selected.enabled = true;
				stickyCost.SetActive(false);
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
			return;
		}

		costText.text = price.ToString();
		_isAvailable = CheckAvailability(price);

		stickyAds.SetActive(!_isAvailable);
		stickyCost.SetActive(_isAvailable);

		costText.color = _isAvailable ? Color.white : cantBuyColor;
		unavailable.gameObject.SetActive(!_isAvailable);
	}

	private static bool CheckAvailability(int price) => price <= ShopStateController.CurrentState.GetState().CoinCount;

	public void ClickOnLocked()
	{
		if (myState != ShopItemState.Locked) return;

		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx

		if (_isAvailable)
		{
			if (_isWeaponItem)
				GameEvents.only.InvokeWeaponSelect(_mySkinIndex, true);
			else
				GameEvents.only.InvokeSkinSelect(_mySkinIndex, true);
			return;
		}

		if (!ApplovinManager.instance) return;

		this.StartListeningForAds();
		
		ApplovinManager.instance.ShowRewardedAds();
	}

	public void ClickOnUnlocked()
	{
		if (_isWeaponItem)
			GameEvents.only.InvokeWeaponSelect(_mySkinIndex, false);
		else
			GameEvents.only.InvokeSkinSelect(_mySkinIndex, false);

		AudioManager.instance.Play("Button");
	}

	public void StartWaiting() => shouldWaitForAd = true;
	public void StopWaiting() => shouldWaitForAd = false;

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		if (_isWeaponItem)
			GameEvents.only.InvokeWeaponSelect(_mySkinIndex, false);
		else
			GameEvents.only.InvokeSkinSelect(_mySkinIndex, false);

		this.StopListeningForAds();
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		this.StopListeningForAds();
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		this.StopListeningForAds();
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		this.StopListeningForAds();
	}
}