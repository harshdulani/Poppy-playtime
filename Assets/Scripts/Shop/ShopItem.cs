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

	private WeaponType _myType;

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

	public void SetWeaponType(WeaponType type) => _myType = type;

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
		print("should be unlocked now");
	}

	public void ClickOnUnlocked()
	{
		print("should be selected now");
	}
}