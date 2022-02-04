using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopController : MonoBehaviour
{
	public static MainShopController shop;
	
	public int[] weaponSkinCosts, armsSkinCosts;
	[SerializeField] private TextMeshProUGUI coinText;

	[SerializeField] private Transform weaponsHolder, armsHolder;
	[SerializeField] private GameObject shopItemPrefab;

	[Header("Arms and Weapons Panels"), SerializeField] private Button weaponsButton;
	[SerializeField] private Button armsButton;
	[SerializeField] private TextMeshProUGUI weaponsText, armsText;
	[SerializeField] private Color black, grey;

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
		ClickWeapons();
	}
	
	public TextMeshProUGUI GetCoinText() => coinText;
	public void UpdateCoinText() => coinText.text = currentState.coinCount.ToString();
	
	private int GetWeaponSkinPrice(int index) => weaponSkinCosts[index];
	private int GetArmsSkinPrice(int index) => armsSkinCosts[index];

	public static int GetWeaponSkinCount() => Enum.GetNames(typeof(WeaponType)).Length;
	public static int GetArmsSkinCount() => Enum.GetNames(typeof(ArmsType)).Length;

	
	private void ReadStoredStateValues(bool initialising = false)
	{
		currentState = initialising
			? StateSaveController.only.LoadSavedState()
			: currentState;

		for (var i = 0; i < GetWeaponSkinCount(); i++)
		{
			var item = initialising 
				? Instantiate(shopItemPrefab, weaponsHolder).GetComponent<ShopItem>() 
				: weaponsHolder.GetChild(i).GetComponent<ShopItem>();

			var itemState = currentState.weaponStates[(WeaponType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(SkinLoader.only.GetWeaponSkinSprite(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(true);
			
			item.SetPriceAndAvailability(GetWeaponSkinPrice(i));
		}

		for (var i = 0; i < GetArmsSkinCount(); i++)
		{
			var item = initialising
				? Instantiate(shopItemPrefab, armsHolder).GetComponent<ShopItem>()
				: armsHolder.GetChild(i).GetComponent<ShopItem>();
			
			var itemState = currentState.armStates[(ArmsType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(SkinLoader.only.GetArmsSkinType(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(false);
			
			item.SetPriceAndAvailability(GetArmsSkinPrice(i));
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
		for (var i = 0; i < GetWeaponSkinCount(); i++)
		{
			if (i == index) continue;

			if (currentState.weaponStates[(WeaponType) i] == ShopItemState.Selected)
				currentState.weaponStates[(WeaponType) i] = ShopItemState.Unlocked;
		}
	
		SaveCurrentShopState();
	}

	public void ChangeSelectedArmsSkin(int index = -1)
	{
		for (var i = 0; i < GetArmsSkinCount(); i++)
		{
			if (i == index) continue;

			if (currentState.armStates[(ArmsType) i] == ShopItemState.Selected)
				currentState.armStates[(ArmsType) i] = ShopItemState.Unlocked;
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
		armsButton.interactable = false;
		armsText.color = grey;
		
		weaponsButton.interactable = true;
		weaponsText.color = black;

		//switch to arms panel
		armsHolder.parent.parent.gameObject.SetActive(true);
		weaponsHolder.parent.parent.gameObject.SetActive(false);
	}

	public void ClickWeapons()
	{
		weaponsButton.interactable = false;
		weaponsText.color = grey;

		armsButton.interactable = true;
		armsText.color = black;

		//switch to weapons panel
		weaponsHolder.parent.parent.gameObject.SetActive(true);
		armsHolder.parent.parent.gameObject.SetActive(false);
	}
}

public enum WeaponType
{
	Punch,
	Hammer,
	Boots,
	Heels,
	Gun,
	Shield,
	Sneaker,
	Pastry
}

public enum ArmsType
{
	Poppy,
	Batman,
	Hulk,
	Spidey,
	Circuits,
	Captain
}