using System;
using UnityEngine;

public class MainShopController : MonoBehaviour
{
	public static MainShopController shop;
	
	public int[] weaponSkinCosts;
	
	[SerializeField] private Transform itemsHolder;
	[SerializeField] private GameObject shopItemPrefab;

	public ShopState currentState;

	private void Awake()
	{
		if (!shop) shop = this;
		else Destroy(gameObject);
		
		ReadStoredStateValues(true);
	}

	public static int GetSkinCount() => Enum.GetNames(typeof(WeaponType)).Length;
	
	private int GetSkinPrice(int index) => weaponSkinCosts[index];
	
	public void ReadStoredStateValues(bool initialising = false)
	{
		currentState = initialising
			? StateSaveController.only.LoadSavedState()
			: currentState;

		for (var i = 0; i < GetSkinCount(); i++)
		{
			var item = initialising 
				? Instantiate(shopItemPrefab, itemsHolder).GetComponent<ShopItem>() 
				: itemsHolder.GetChild(i).GetComponent<ShopItem>();

			var itemState = currentState.weaponStates[(WeaponType) i];
			item.SetWeaponType(i);
			item.SetState(itemState);
			item.SetIconSprite(SkinLoader.only.GetSkinSprite(i, itemState == ShopItemState.Locked));
			
			item.SetPriceAndAvailability(GetSkinPrice(i));
		}
	}
	
	// make sure all instances of coinShop._currentSkin are tied over here
	// make sure whenever currentskin is updated, state is saved
	// when you buy a skin, its state goes to unlocked
	// when you click on a unlocked button, state updates to selected
	// all this should be reflected upon in shop item as well as coinshop

	public void SaveCurrentShopState()
	{
		StateSaveController.only.SaveCurrentState(currentState);
		ReadStoredStateValues();
	}

	public void ChangeSelectedWeapon(int index = -1)
	{
		for (var i = 0; i < GetSkinCount(); i++)
		{
			if (i == index) continue;

			if (currentState.weaponStates[(WeaponType) i] == ShopItemState.Selected)
				currentState.weaponStates[(WeaponType) i] = ShopItemState.Unlocked;
		}
	
		SaveCurrentShopState();
	}
}