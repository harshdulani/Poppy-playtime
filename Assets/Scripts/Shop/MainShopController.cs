using System;
using UnityEngine;

public class MainShopController : MonoBehaviour
{
	public static MainShopController shop;
	
	public int[] weaponSkinCosts;
	
	[SerializeField] private Transform itemsHolder;
	[SerializeField] private GameObject shopItemPrefab;

	public ShopState currentState;
	
	private Animator _anim;

	private bool _viewingWeaponsSkins = true;
	
	private static readonly int Close = Animator.StringToHash("Close");
	private static readonly int Open = Animator.StringToHash("Open");

	private void Awake()
	{
		if (!shop) shop = this;
		else Destroy(gameObject);
		
		ReadStoredStateValues(true);
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public static int GetSkinCount() => Enum.GetNames(typeof(WeaponType)).Length;
	
	private int GetSkinPrice(int index) => weaponSkinCosts[index];

	private void ReadStoredStateValues(bool initialising = false)
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

	public void OpenShop()
	{
		_anim.SetTrigger(Open);
	}

	public void CloseShop()
	{
		_anim.SetTrigger(Close);
	}

	public void ClickArms()
	{
		
	}

	public void ClickWeapons()
	{
		
	}
}