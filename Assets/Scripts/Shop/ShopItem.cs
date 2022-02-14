using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
	Locked, Unlocked, Selected
}
public class ShopItem : MonoBehaviour
{
	[SerializeField] private ShopItemState myState;
	[SerializeField] private Image unlocked, selected, icon, unavailable;
	[SerializeField] private TextMeshProUGUI costText, questionMark;
	[SerializeField] private Color cantBuyColor;

	[SerializeField] private Mask iconMask;
	[SerializeField] private bool shouldUseMask;
	
	private bool _isWeaponItem;
	private bool _isAvailable;
	private int _mySkinIndex;

	public void SetIconSprite(Sprite image)
	{
		iconMask.graphic.enabled = shouldUseMask;
		if(image)
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
				costText.transform.parent.gameObject.SetActive(true);
				break;
			case ShopItemState.Unlocked:
				unlocked.enabled = true;
				selected.enabled = false;
				costText.transform.parent.gameObject.SetActive(false);
				break;
			case ShopItemState.Selected:
				unlocked.enabled = false;
				selected.enabled = true;
				costText.transform.parent.gameObject.SetActive(false);
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
			costText.gameObject.SetActive(false);
			unavailable.gameObject.SetActive(false);
			return;
		}
		
		costText.text = price.ToString(); 
		_isAvailable = CheckAvailability(price);

		costText.color = _isAvailable ? Color.white : cantBuyColor;

		unavailable.gameObject.SetActive(!_isAvailable);
	}

	private bool CheckAvailability(int price) => price <= ShopStateController.CurrentState.GetState().CoinCount;

	public void ClickOnLocked()
	{
		// if(ApplovinManager.instance)
		// 	ApplovinManager.instance.ShowRewardedAds();
		
		if(myState != ShopItemState.Locked) return;
		
		if(_isWeaponItem)
			GameEvents.only.InvokeWeaponSelect(_mySkinIndex, true);
		else
			GameEvents.only.InvokeSkinSelect(_mySkinIndex, true);
		
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void ClickOnUnlocked()
	{
		if(_isWeaponItem)
			GameEvents.only.InvokeWeaponSelect(_mySkinIndex, false);
		else
			GameEvents.only.InvokeSkinSelect(_mySkinIndex, false);
		
		AudioManager.instance.Play("Button");
	}
}