using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopController : MonoBehaviour, IWantsAds
{
	private enum AdRewardType
	{
		None,
		OneFifty,
		ThreeHundred
	}
	
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

	private AdRewardType _currentRewardType;

	#region Animator Hashes

	private static readonly int Close = Animator.StringToHash("Close");
	private static readonly int Open = Animator.StringToHash("Open");
	private static readonly int ShowShopButton = Animator.StringToHash("showShopButton");
	private static readonly int HideShopButton = Animator.StringToHash("hideShopButton");
	#endregion
		
	#region Helpers and Getters
	public TextMeshProUGUI GetCoinText() => coinText;
	public void UpdateCoinText() => coinText.text = ShopStateController.CurrentState.GetState().CoinCount.ToString();
	
	public static int GetWeaponSkinCount() => Enum.GetNames(typeof(WeaponType)).Length;
	public static int GetArmsSkinCount() => Enum.GetNames(typeof(ArmsType)).Length;

	public Sprite GetWeaponSprite(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackWeaponSprites : coloredWeaponSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}

	public Sprite GetArmsSkinSprite(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackArmSprites : coloredArmSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}
	
	private int GetWeaponSkinPrice(int index) => weaponSkinCosts[index];
	private int GetArmsSkinPrice(int index) => armsSkinCosts[index];
	#endregion
	
	private void OnEnable()
	{
		GameEvents.Only.WeaponSelect += OnWeaponPurchase;
		GameEvents.Only.SkinSelect += OnSkinPurchase;
		
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.GameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.Only.WeaponSelect -= OnWeaponPurchase;
		GameEvents.Only.SkinSelect -= OnSkinPurchase;
		
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		GameEvents.Only.GameEnd -= OnGameEnd;
	}

	private void OnDestroy() => AdsMediator.StopListeningForAds(this);

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
			
			//if you are having an index out of bounds error over here, check if prices and colored & black sprites have equal no of items
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
			item.SetIconSprite(GetArmsSkinSprite(i, itemState == ShopItemState.Locked));
			item.SetIsWeaponItem(false);
			
			//if you are having an index out of bounds error over here, check if prices and colored & black sprites have equal no of items
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
		for (var i = 0; i < GetWeaponSkinCount(); i++)
		{
			if (i == index) continue;

			if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] == ShopItemState.Selected)
				ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] = ShopItemState.Unlocked;
		}
	}

	private void ChangeSelectedArmsSkin(int index = -1)
	{
		for (var i = 0; i < GetArmsSkinCount(); i++)
		{
			if (i == index) continue;
			
			if (ShopStateController.CurrentState.GetState().armStates[(ArmsType) i] == ShopItemState.Selected)
				ShopStateController.CurrentState.GetState().armStates[(ArmsType) i] = ShopItemState.Unlocked;
		}
	}

	public void OpenShop()
	{
		if(!_canClick) return;
		
		_anim.SetTrigger(Open);
		_anim.SetTrigger(HideShopButton);
		InputHandler.AssignTemporaryDisabledState();
	}

	public void CloseShop()
	{
		_anim.SetTrigger(Close);
		_anim.SetTrigger(ShowShopButton);
		InputHandler.Only.AssignIdleState();
		SidebarShopController.only.UpdateButtons();
	}

	public void ClickArms()
	{
		if (ApplovinManager.instance) 
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		
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
		if (ApplovinManager.instance) 
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		
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
	
	public void ClickExtraCoins_150()
	{
		if(!ApplovinManager.instance) return;
		if(!ApplovinManager.instance.TryShowRewardedAds()) return;

		StartWaiting(AdRewardType.OneFifty);
		AdsMediator.StartListeningForAds(this);
	}
	public void ClickExtraCoins_300()
	{
		if(!ApplovinManager.instance) return;
		if(!ApplovinManager.instance.TryShowRewardedAds()) return;

		StartWaiting(AdRewardType.ThreeHundred);
		AdsMediator.StartListeningForAds(this);
	}
	
	private void OnTapToPlay()
	{
		_anim.SetTrigger(HideShopButton);
		_canClick = false;
		coinText.transform.parent.gameObject.SetActive(false);
	}

	private void OnGameEnd()
	{
		coinText.transform.parent.gameObject.SetActive(true);
	}

	private void StartWaiting(AdRewardType newType) => _currentRewardType = newType;

	private void StopWaiting() => _currentRewardType = AdRewardType.None;

	private void AdRewardReceiveBehaviour()
	{
		switch (_currentRewardType)
		{
			case AdRewardType.None:
				break;
			case AdRewardType.OneFifty:
				SidebarShopController.AlterCoinCount(150);
				SidebarShopController.only.CoinsGoingUpEffect();
				break;
			case AdRewardType.ThreeHundred:
				SidebarShopController.AlterCoinCount(300);
				SidebarShopController.only.CoinsGoingUpEffect();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		AdRewardReceiveBehaviour();
	}

	public void OnShowDummyAd()
	{
		AdRewardReceiveBehaviour();
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		StopWaiting();
		AdsMediator.StopListeningForAds(this);
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
	Poop,
	Flowers,
	Phone,
	IceCream
}

public enum ArmsType
{
	Skin,
	Spidey,
	Thanos,
	Captain,
	IronMan,
	Batman,
	Hulk,
	Poppy
}