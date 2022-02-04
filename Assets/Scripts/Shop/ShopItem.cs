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
			icon.sprite = image;
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

	private bool CheckAvailability(int price) => price <= MainShopController.shop.currentState.coinCount;

	public void ClickOnLocked()
	{
		if(myState != ShopItemState.Locked) return;
		
		//Decrease coin count
		MainShopController.shop.currentState.coinCount -= _isWeaponItem
			? MainShopController.shop.weaponSkinCosts[_mySkinIndex]
			: MainShopController.shop.armsSkinCosts[_mySkinIndex];
		
		MainShopController.shop.UpdateCoinText();
		
		ChangeSelected();
		
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void ClickOnUnlocked()
	{
		ChangeSelected();

		AudioManager.instance.Play("Button");
	}

	private void ChangeSelected()
	{
		if(_isWeaponItem)
		{
			//Change state of item to selected
			MainShopController.shop.currentState.weaponStates[(WeaponType) _mySkinIndex] = ShopItemState.Selected;
					
			//make sure nobody else is selected
			MainShopController.shop.ChangeSelectedWeapon(_mySkinIndex);
			
			//Reflect changes to come in skin loader & coin shop UI and also in hand
			SkinLoader.only.UpdateWeaponSkinInUse(_mySkinIndex);
			InputHandler.Only.GetRightHand().UpdateEquippedWeaponsSkin(false);
			
			transform.root.GetComponentInChildren<CoinShopController>().UpdateButtons();
		}
		else
		{
			//Change state of item to selected
			MainShopController.shop.currentState.armStates[(ArmsType) _mySkinIndex] = ShopItemState.Selected;
			
			//make sure nobody else is selected
			MainShopController.shop.ChangeSelectedArmsSkin(_mySkinIndex);
			
			//Reflect changes to come in skin loader & coin shop UI and also in hand
			SkinLoader.only.UpdateArmsSkinInUse(_mySkinIndex);
			InputHandler.Only.GetLeftHand().UpdateEquippedArmsSkin();
			InputHandler.Only.GetRightHand().UpdateEquippedArmsSkin();
		}
	}
}