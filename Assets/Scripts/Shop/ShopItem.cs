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
	//Working here, add interactable = false is unavailable
	//Skinloader has a get item count, make it so that it is fool proof and hence more attractively callable
	[SerializeField] private ShopItemState myState;
	private bool _isAvailable;
	
	[SerializeField] private Image unlocked, selected, icon, unavailable;
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private Color cantBuyColor;

	private int _myWeaponTypeIndex;

	public void SetIconSprite(Sprite image) => icon.sprite = image;

	public void SetState(ShopItemState state)
	{
		myState = state;
		switch (myState)
		{
			case ShopItemState.Locked:
				unlocked.enabled = false;
				selected.enabled = false;
				break;
			case ShopItemState.Unlocked:
				unlocked.enabled = true;
				selected.enabled = false;
				break;
			case ShopItemState.Selected:
				unlocked.enabled = false;
				selected.enabled = true;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		unavailable.gameObject.SetActive(false);
	}

	public void SetWeaponType(int idx) => _myWeaponTypeIndex = idx;

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
		//Decrease coin count
		MainShopController.shop.currentState.coinCount -= MainShopController.shop.weaponSkinCosts[_myWeaponTypeIndex];
		//Change state of weapon to selected
		MainShopController.shop.currentState.weaponStates[(WeaponType) _myWeaponTypeIndex] = ShopItemState.Selected;
		//make sure nobody else is selected
		MainShopController.shop.ChangeSelectedWeapon(_myWeaponTypeIndex);
		
		//Reflect changes to come in skin loader & coin shop UI and also in hand
		SkinLoader.only.UpdateSkinInUse(_myWeaponTypeIndex);
		InputHandler.Only.GetRightHand().UpdateEquippedSkin(false);
		transform.root.GetComponentInChildren<CoinShopController>().UpdateButtons();
		
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void ClickOnUnlocked()
	{
		//Change state of weapon to selected
		MainShopController.shop.currentState.weaponStates[(WeaponType) _myWeaponTypeIndex] = ShopItemState.Selected;
		//make sure nobody else is selected
		MainShopController.shop.ChangeSelectedWeapon(_myWeaponTypeIndex);
		
		//Reflect changes to come in skin loader & coin shop UI and also in hand
		SkinLoader.only.UpdateSkinInUse(_myWeaponTypeIndex);
		InputHandler.Only.GetRightHand().UpdateEquippedSkin(false);
		transform.root.GetComponentInChildren<CoinShopController>().UpdateButtons();
		
		AudioManager.instance.Play("Button");
	}
}