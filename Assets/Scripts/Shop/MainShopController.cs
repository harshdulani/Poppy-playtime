using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopController : MonoBehaviour
{
	public int[] weaponSkinCosts, armsSkinCosts;
	[SerializeField] private TextMeshProUGUI coinText;

	[SerializeField] private GridLayoutGroup weaponsHolder, armsHolder;
	[SerializeField] private GameObject shopItemPrefab;
	
	[Header("Arms and Weapons Panels"), SerializeField] private Button weaponsButton;
	[SerializeField] private Button armsButton;
	[SerializeField] private TextMeshProUGUI weaponsText, armsText;
	[SerializeField] private Color black, grey;
	
	public ShopState currentState;
	
	private const float HitCooldown = 1f;
	private Animator _anim;
	
	private static readonly int Close = Animator.StringToHash("Close");
	private static readonly int Open = Animator.StringToHash("Open");
	private static readonly int ShowShopButton = Animator.StringToHash("showShopButton");
	private static readonly int HideShopButton = Animator.StringToHash("hideShopButton");
	private bool _canClick = true;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
	}

	private void Awake()
	{
		ReadStoredStateValues(true);
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_anim.SetTrigger(ShowShopButton);
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

		var weaponCount = GetWeaponSkinCount();
		var armsCount = GetArmsSkinCount();
		
		var weaponRect = weaponsHolder.GetComponent<RectTransform>();
		weaponRect.sizeDelta = new Vector2(weaponRect.sizeDelta.x,
			Mathf.CeilToInt(weaponCount / (float) weaponsHolder.constraintCount) * (weaponsHolder.cellSize.y + weaponsHolder.spacing.y));

		var armsRect = armsHolder.GetComponent<RectTransform>();
		armsRect.sizeDelta = new Vector2(armsRect.sizeDelta.x,
			Mathf.CeilToInt(armsCount / (float) armsHolder.constraintCount) * (armsHolder.cellSize.y + 2 * armsHolder.spacing.y));
		
		for (var i = 0; i < weaponCount; i++)
		{
			var item = initialising 
				? Instantiate(shopItemPrefab, weaponsHolder.transform).GetComponent<ShopItem>() 
				: weaponsHolder.transform.GetChild(i).GetComponent<ShopItem>();

			var itemState = currentState.weaponStates[(WeaponType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(ShopReferences.refs.skinLoader.GetWeaponSkinSprite(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(true);
			item.SetPriceAndAvailability(GetWeaponSkinPrice(i));
		}

		for (var i = 0; i < armsCount; i++)
		{
			var item = initialising
				? Instantiate(shopItemPrefab, armsHolder.transform).GetComponent<ShopItem>()
				: armsHolder.transform.GetChild(i).GetComponent<ShopItem>();
			
			var itemState = currentState.armStates[(ArmsType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(ShopReferences.refs.skinLoader.GetArmsSkinType(i, itemState == ShopItemState.Locked));
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

	private void ClickCooldown() => _canClick = true;
	
	public void OpenShop()
	{
		if(!_canClick) return;
		
		_anim.SetTrigger(Open);
		_anim.SetTrigger(HideShopButton);
		_canClick = false;
		DOVirtual.DelayedCall(HitCooldown, ClickCooldown);
	}

	public void CloseShop()
	{		
		if(!_canClick) return;

		_anim.SetTrigger(Close);
		_anim.SetTrigger(ShowShopButton);
		_canClick = false;
		DOVirtual.DelayedCall(HitCooldown, ClickCooldown);
	}

	public void ClickArms()
	{
		armsButton.interactable = false;
		armsText.color = grey;
		
		weaponsButton.interactable = true;
		weaponsText.color = black;

		//switch to arms panel
		armsHolder.transform.parent.parent.gameObject.SetActive(true);
		weaponsHolder.transform.parent.parent.gameObject.SetActive(false);
	}

	public void ClickWeapons()
	{
		weaponsButton.interactable = false;
		weaponsText.color = grey;

		armsButton.interactable = true;
		armsText.color = black;

		//switch to weapons panel
		weaponsHolder.transform.parent.parent.gameObject.SetActive(true);
		armsHolder.transform.parent.parent.gameObject.SetActive(false);
	}

	private void OnTapToPlay()
	{
		_anim.SetTrigger(HideShopButton);
		_canClick = false;
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
	Pastry,
	Burger,
	Poop
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