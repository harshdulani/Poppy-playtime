using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopController : MonoBehaviour
{
	public static MainShopController Main;
	
	[SerializeField] private Sprite[] coloredWeaponSprites, blackWeaponSprites;
	[SerializeField] private Sprite[] coloredArmSprites, blackArmSprites;
	
	[Header("Coins and costs"), SerializeField] private TextMeshProUGUI coinText;
	public int[] weaponSkinCosts, armsSkinCosts;

	[SerializeField] private GridLayoutGroup weaponsHolder, armsHolder;
	[SerializeField] private GameObject shopItemPrefab;
	
	[Header("Arms and Weapons Panels"), SerializeField] private Button weaponsButton;
	[SerializeField] private Button armsButton;
	[SerializeField] private TextMeshProUGUI weaponsText, armsText;
	[SerializeField] private Color black, grey;
	
	private Animator _anim;
	private bool _canClick = true;

	#region Animator Hashes
	private static readonly int Close = Animator.StringToHash("Close");
	private static readonly int Open = Animator.StringToHash("Open");
	private static readonly int ShowShopButton = Animator.StringToHash("showShopButton");
	private static readonly int HideShopButton = Animator.StringToHash("hideShopButton");
	#endregion
		
	#region Helpers and Getters
	public TextMeshProUGUI GetCoinText() => coinText;
	public void UpdateCoinText() => coinText.text = ShopStateController.CurrentState.GetState().CoinCount.ToString();
	
	private int GetWeaponSkinPrice(int index) => weaponSkinCosts[index];
	private int GetArmsSkinPrice(int index) => armsSkinCosts[index];

	public static int GetWeaponSkinCount() => Enum.GetNames(typeof(WeaponType)).Length;
	public static int GetArmsSkinCount() => Enum.GetNames(typeof(ArmsType)).Length;
	
	public Sprite GetWeaponSprite(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackWeaponSprites : coloredWeaponSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}

	private Sprite GetArmsSkinType(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackArmSprites : coloredArmSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}
	#endregion
	
	private void OnEnable()
	{
		GameEvents.only.weaponSelect += OnWeaponPurchase;
		GameEvents.only.skinSelect += OnSkinPurchase;
		
		GameEvents.only.tapToPlay += OnTapToPlay;
	}

	private void OnDisable()
	{
		GameEvents.only.weaponSelect -= OnWeaponPurchase;
		GameEvents.only.skinSelect -= OnSkinPurchase;
		
		GameEvents.only.tapToPlay -= OnTapToPlay;
	}
	
	private void Awake()
	{
		if (!Main) Main = this;
		else Destroy(gameObject);
		
		ReadCurrentShopState(true);
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_anim.SetTrigger(ShowShopButton);
		ClickWeapons();
		
		weaponsHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		armsHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
	}

	/// <summary>
	/// This function not only reads the change of the values, but also sets all the ShopItems to the visual and behavioural representation they should be in.
	/// </summary>
	/// <param name="initialising"> Optional parameter. Only set to true when calling to initialise values/ generate scroll views.</param>
	public void ReadCurrentShopState(bool initialising = false)
	{
		var currentShopState = initialising
			? ShopStateController.ShopStateSerializer.LoadSavedState()
			: ShopStateController.CurrentState.GetState();

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

			var itemState = currentShopState.weaponStates[(WeaponType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(GetWeaponSprite(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(true);
			item.SetPriceAndAvailability(GetWeaponSkinPrice(i));
		}

		for (var i = 0; i < armsCount; i++)
		{
			var item = initialising
				? Instantiate(shopItemPrefab, armsHolder.transform).GetComponent<ShopItem>()
				: armsHolder.transform.GetChild(i).GetComponent<ShopItem>();
			
			var itemState = currentShopState.armStates[(ArmsType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			item.SetIconSprite(GetArmsSkinType(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(false);
			
			item.SetPriceAndAvailability(GetArmsSkinPrice(i));
		}
		
		UpdateCoinText();
	}

	private void SaveCurrentShopState()
	{
		//save the newest made change of state
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		
		//make shop items represent their state acc to new change of state 
		ReadCurrentShopState();
	}

	private void ChangeSelectedWeapon(int index = -1)
	{
		var weaponStates = ShopStateController.CurrentState.GetWeaponStates();
		for (var i = 0; i < GetWeaponSkinCount(); i++)
		{
			if (i == index) continue;

			if (weaponStates[(WeaponType) i] == ShopItemState.Selected)
				weaponStates[(WeaponType) i] = ShopItemState.Unlocked;
		}
		SaveCurrentShopState();
	}

	private void ChangeSelectedArmsSkin(int index = -1)
	{
		var armStates = ShopStateController.CurrentState.GetSkinStates();
		for (var i = 0; i < GetArmsSkinCount(); i++)
		{
			if (i == index) continue;

			if (armStates[(ArmsType) i] == ShopItemState.Selected)
				armStates[(ArmsType) i] = ShopItemState.Unlocked;
		}
		SaveCurrentShopState();
	}

	public void OpenShop()
	{
		if(!_canClick) return;
		
		_anim.SetTrigger(Open);
		_anim.SetTrigger(HideShopButton);
		InputHandler.Only.AssignDisabledState();
	}

	public void CloseShop()
	{
		_anim.SetTrigger(Close);
		_anim.SetTrigger(ShowShopButton);
		InputHandler.Only.AssignIdleState();
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
	
	private void OnWeaponPurchase(int index, bool shouldDeductCoins)
	{
		if(shouldDeductCoins)
		{
			//if was locked before this, Decrease coin count
			ShopStateController.CurrentState.GetState().CoinCount -= weaponSkinCosts[index];
			UpdateCoinText();
		} 

		//mark purchased weapon as selected
		ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) index] = ShopItemState.Selected;
		
		//make sure nobody else is selected/ old one is now marked as unlocked
		ChangeSelectedWeapon(index);
		
		
		//Save the state and reflect it in Shop UI
		SaveCurrentShopState();
	}
	
	private void OnSkinPurchase(int index, bool shouldDeductCoins)
	{
		if(shouldDeductCoins)
		{
			//if was locked before this, Decrease coin count
			ShopStateController.CurrentState.GetState().CoinCount -= armsSkinCosts[index];
			UpdateCoinText();
		}
		
		//mark purchased skin as selected
		ShopStateController.CurrentState.GetState().armStates[(ArmsType) index] = ShopItemState.Selected;
		
		//make sure nobody else is selected/ old one is now marked as unlocked
		ChangeSelectedArmsSkin(index);
		
		//Save the state and reflect it in Shop UI
		SaveCurrentShopState();
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