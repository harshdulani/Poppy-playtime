using System;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SidebarShopController : MonoBehaviour//, IWantsAds
{
	private enum AdRewardTypes
	{
		None,
		Speed,
		Power,
		Weapon
	}
	
	public static SidebarShopController only;

	[SerializeField] private Sprite VideoBtn, normalBtn;
	[SerializeField] private int[] speedLevelCosts, powerLevelCosts;
	[SerializeField] private Button speedButton, powerButton, skinButton;
	[SerializeField] private GameObject speedHand, powerHand, skinHand;
	[SerializeField] private Image currentSkinImage;
	[SerializeField] private TextMeshProUGUI speedMultiplier, speedCostText, powerMultiplier, powerCostText, skinName, skinCostText;
	[SerializeField] private Animation speedButtonPressAnimation, powerButtonPressAnimation, skinButtonPressAnimation;

	private const float CooldownTimerDuration = 0.25f;
	private bool _allowedToPressButton = true;

	[Header("Coin Particle Effect"), SerializeField] private RectTransform coinHolder;
	[SerializeField] private ParticleControlScript coinParticles;

	private Animation _anim;
	private int _currentSpeedLevel, _currentPowerLevel;
	
	private AdRewardTypes _currentRewardType;
	
	private static int GetSidebarWeapon() => ShopStateController.CurrentState.GetState().SidebarWeapon;

	public static void AlterCoinCount(int change)
	{
		ShopStateController.CurrentState.GetState().CoinCount += change;
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.UpdateCoinText();
		MainShopController.Main.ReadCurrentShopState();
	}

	private static int GetCoinCount() => ShopStateController.CurrentState.GetState().CoinCount;

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
	}

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		
		GameEvents.Only.WeaponSelect += OnWeaponPurchase;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		
		GameEvents.Only.WeaponSelect -= OnWeaponPurchase;
	}

	//private void OnDestroy() => AdsMediator.StopListeningForAds(this);

	private void Start()
	{
		_anim = GetComponent<Animation>();

		_currentSpeedLevel = ShopStateController.CurrentState.GetCurrentSpeedLevel();
		_currentPowerLevel = ShopStateController.CurrentState.GetCurrentPowerLevel();

		UpdateButtons();
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.O)) return;

		AlterCoinCount(500);
		UpdateButtons();
	}

	private static void FindNewSideBarWeapon(int currentWeapon)
	{
		var changed = false;
		
		//find a weapon from current index to last
		for (var i = currentWeapon + 1; i < MainShopController.GetWeaponSkinCount(); i++)
		{
			if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] != ShopItemState.Locked)
				continue;

			ShopStateController.CurrentState.SetNewSideBarWeapon(i);
			changed = true;
			break;
		}

		//if all weapons after me are unlocked, try to find new before me
		if (!changed)
		{
			for (var i = 1; i < currentWeapon; i++)
			{
				if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] != ShopItemState.Locked)
					continue;

				ShopStateController.CurrentState.SetNewSideBarWeapon(i);
				changed = true;
				break;
			}
		}

		//if still didn't find anything make sure "MAX" is written
		if (!changed)
			ShopStateController.CurrentState.AllWeaponsHaveBeenUnlocked();
	}

	public void UpdateButtons()
	{
		//update speed and power texts and icons
		if(_currentSpeedLevel < speedLevelCosts.Length - 1)
		{
			speedMultiplier.text = "Speed: x" + (_currentSpeedLevel + 1);
			speedCostText.text = speedLevelCosts[_currentSpeedLevel + 1].ToString();
			
			speedButton.interactable = GetCoinCount() >= speedLevelCosts[_currentSpeedLevel + 1];
			if (GetCoinCount() < speedLevelCosts[_currentSpeedLevel + 1])
			{
				//if(ApplovinManager.instance && ApplovinManager.instance.enableAds)
				if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
				{
					speedButton.image.sprite = VideoBtn;
					speedCostText.gameObject.SetActive(false);
				}
				else
				{
					speedButton.image.sprite = normalBtn;
					speedCostText.gameObject.SetActive(true);
				}
			}
			else
			{
				speedButton.image.sprite = normalBtn;
				speedCostText.gameObject.SetActive(true);
			}
		}
		else
		{
			speedCostText.text = "MAX";
			speedMultiplier.text = "MAX";
			speedButton.interactable = false;
		}
		speedHand.SetActive(speedButton.interactable);
		
		if(_currentPowerLevel < powerLevelCosts.Length - 1)
		{
			powerMultiplier.text = "Power: x" + (_currentPowerLevel + 1);
			powerCostText.text = powerLevelCosts[_currentPowerLevel + 1].ToString();
			
			powerButton.interactable = GetCoinCount() >= powerLevelCosts[_currentPowerLevel + 1];
			if (GetCoinCount() < powerLevelCosts[_currentPowerLevel + 1])
			{
				//if(ApplovinManager.instance && ApplovinManager.instance.enableAds)
				if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
				{
					powerButton.image.sprite = VideoBtn;
					powerCostText.gameObject.SetActive(false);
				}
				else
				{
					powerButton.image.sprite = normalBtn;
					powerButton.interactable = false;
					powerCostText.gameObject.SetActive(true);
				}
			}
			else
			{
				powerButton.image.sprite = normalBtn;
				powerCostText.gameObject.SetActive(true);
			}
		}
		else
		{
			powerCostText.text = "MAX";
			powerMultiplier.text = "MAX";
			powerButton.interactable = false;
		}
		powerHand.SetActive(powerButton.interactable);

		if (ShopStateController.CurrentState.AreAllWeaponsUnlocked())
		{
			skinName.text = "MAX";
			skinCostText.text = "MAX";
			skinButton.interactable = false;
		}
		else
		{
			skinName.text = ((WeaponType) GetSidebarWeapon()).ToString();
			skinCostText.text = MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()].ToString();
			
			skinButton.interactable = GetCoinCount() >= MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()];
			if (GetCoinCount() < MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()])
			{
				//if(ApplovinManager.instance && ApplovinManager.instance.enableAds)
				if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
				{
					skinButton.image.sprite = VideoBtn;
					skinCostText.gameObject.SetActive(false);
				}
				else
				{
					skinButton.image.sprite = normalBtn;
					skinCostText.gameObject.SetActive(true);
				}
			}
			else
			{
				skinButton.image.sprite = normalBtn;
				skinCostText.gameObject.SetActive(true);
			}
		}
		
		skinHand.SetActive(skinButton.interactable);
		
		currentSkinImage.sprite = MainShopController.Main.GetWeaponSprite(GetSidebarWeapon());
	}

	public void ClickOnBuySpeed()
	{
		if(!_allowedToPressButton) return;
		
		if (GetCoinCount() < speedLevelCosts[_currentSpeedLevel + 1])
		{
			/*
			if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds || !ApplovinManager.instance.TryShowRewardedAds()) return;

			StartWaiting(AdRewardTypes.Speed);
			AdsMediator.StartListeningForAds(this);
			*/
			
			if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
			{
				StartWaiting(AdRewardTypes.Speed);
				YcHelper.ShowRewardedAds(AdRewardReceiveBehaviour);
			}
		}
		else
			BuySpeed();

		//if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds) return;
		if (!YcHelper.InstanceExists || !YcHelper.IsAdAvailable()) return;
			
		_allowedToPressButton = speedButton.interactable = false;
		DOVirtual.DelayedCall(CooldownTimerDuration, () => _allowedToPressButton = speedButton.interactable = true);
	}
	
	public void ClickOnBuyPower()
	{
		if(!_allowedToPressButton) return;
		
		if (GetCoinCount() < powerLevelCosts[_currentPowerLevel + 1])
		{
			/*
			if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds || !ApplovinManager.instance.TryShowRewardedAds()) return;

			StartWaiting(AdRewardTypes.Power);
			AdsMediator.StartListeningForAds(this);
			*/
			
			if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
			{
				StartWaiting(AdRewardTypes.Power);
				YcHelper.ShowRewardedAds(AdRewardReceiveBehaviour);
			}
		}
		else
			BuyPower();
		
		//if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds) return;
		if (!YcHelper.InstanceExists || !YcHelper.IsAdAvailable()) return;
		
		_allowedToPressButton = powerButton.interactable = false;
		DOVirtual.DelayedCall(CooldownTimerDuration, () => _allowedToPressButton = powerButton.interactable = true);
	}

	public void ClickOnBuyNewWeapon()
	{
		if(!_allowedToPressButton) return;
		
		if (GetCoinCount() < MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()])
		{
			/*
			if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds || !ApplovinManager.instance.TryShowRewardedAds()) return;

			StartWaiting(AdRewardTypes.Weapon);
			AdsMediator.StartListeningForAds(this);
			*/
			if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
			{
				StartWaiting(AdRewardTypes.Weapon);
				YcHelper.ShowRewardedAds(AdRewardReceiveBehaviour);
			}
		}
		else
			BuyWeapon();
		
		//if(!ApplovinManager.instance || !ApplovinManager.instance.enableAds) return;
		if (!YcHelper.InstanceExists || !YcHelper.IsAdAvailable()) return;
		
		_allowedToPressButton = skinButton.interactable = false;
		DOVirtual.DelayedCall(CooldownTimerDuration, () => _allowedToPressButton = skinButton.interactable = true);
	}

	private void BuySpeed(bool usingAds = false)
	{
		speedButtonPressAnimation.Play();
		if (usingAds)
		{
			StopWaiting();
			//AdsMediator.StopListeningForAds(this);
			_currentSpeedLevel++;
		}
		else
			AlterCoinCount(-speedLevelCosts[++_currentSpeedLevel]);
		
		InputHandler.Only.GetLeftHand().UpdatePullingSpeed(_currentSpeedLevel);
		ShopStateController.CurrentState.SetNewSpeedLevel(_currentSpeedLevel);
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.ReadCurrentShopState();
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
	}

	private void BuyPower(bool usingAds = false)
	{
		powerButtonPressAnimation.Play();
		
		if (usingAds)
		{
			StopWaiting();
			//AdsMediator.StopListeningForAds(this);
			_currentPowerLevel++;
		}
		else
			AlterCoinCount(-powerLevelCosts[++_currentPowerLevel]);
		
		//ABSOLUTELY NO change in power script
		ShopStateController.CurrentState.SetNewPowerLevel(_currentPowerLevel);
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.ReadCurrentShopState();

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	private void BuyWeapon(bool usingAds = false)
	{
		if (usingAds)
		{
			StopWaiting();
			//AdsMediator.StopListeningForAds(this);
		}
		
		skinButtonPressAnimation.Play();
		//deduct money if not buying through ads
		GameEvents.Only.InvokeWeaponSelect(GetSidebarWeapon(), !usingAds);
		AudioManager.instance.Play("Button");
	}

	private void OnWeaponPurchase(int index, bool shouldDeductCoins)
	{
		if(index != GetSidebarWeapon()) return;
		
		FindNewSideBarWeapon(GetSidebarWeapon());

		DOVirtual.DelayedCall(0.25f, UpdateButtons);
	}
	
	private void OnTapToPlay() => _anim.Play();

	public void IncreaseCoinsBy(int coinIncreaseCount)
	{
		var seq = DOTween.Sequence();
		seq.AppendInterval(1.25f);

		var coinText = MainShopController.Main.GetCoinText();
		var initSize = MainShopController.Main.GetCoinText().fontSize;

		var dummyCoinCount = GetCoinCount();

		AudioManager.instance.Play("CoinCollect");
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize * 1.2f, .5f).SetEase(Ease.OutQuart));
		seq.Join(DOTween.To(() => dummyCoinCount, value => dummyCoinCount = value, dummyCoinCount + coinIncreaseCount,
			.5f).OnUpdate(() => coinText.text = dummyCoinCount.ToString()));
		seq.InsertCallback(.75f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() => AlterCoinCount(coinIncreaseCount));
	}

	public void CoinsGoingUpEffect()
	{
		coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder);
	}

	private void StartWaiting(AdRewardTypes newType) => _currentRewardType = newType;
	private void StopWaiting() => _currentRewardType = AdRewardTypes.None;

	private void AdRewardReceiveBehaviour()
	{
		switch (_currentRewardType)
		{
			case AdRewardTypes.None:
				break;
			case AdRewardTypes.Speed:
				BuySpeed(true);
				break;
			case AdRewardTypes.Power:
				BuyPower(true);
				break;
			case AdRewardTypes.Weapon:
				BuyWeapon(true);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		StopWaiting();
		//AdsMediator.StopListeningForAds(this);
	}
	
	/*
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
	}*/
}