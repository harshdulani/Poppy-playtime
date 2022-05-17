using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinLoader : MonoBehaviour, IWantsAds
{
	private enum AdRewardType
	{
		None,
		CoinMultiplier,
		NewWeapon
	}

	[SerializeField] private RectTransform barPivot;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage, blackBackground;
	
	[SerializeField] private Button skipButton, getItButton, claimButton;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText, claimMulTxt;

	[SerializeField] private GameObject loaderPanel, unlockedButtonsHolder, percentageUI; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait, skipButtonWait;
	
	[SerializeField] private int coinIncreaseCount = 100;
	
	private MainCanvasController _mainCanvas;
	private float _currentSkinPercentageUnlocked;
	
	private AdRewardType _currentAdRewardType;
	
	private static int GetLoaderWeapon() => ShopStateController.CurrentState.GetState().LoaderWeapon;

	private void OnEnable()
	{
		GameEvents.Only.WeaponSelect += OnWeaponPurchase;
		
		GameEvents.Only.GameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.Only.WeaponSelect -= OnWeaponPurchase;
		
		GameEvents.Only.GameEnd -= OnGameEnd;
	}

	private void OnDestroy() => AdsMediator.StopListeningForAds(this);

	private void Start()
	{
		_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>();
		
		Initialise();
		
		loaderPanel.SetActive(false);
		//skipButton.interactable = false;
		skipButton.gameObject.SetActive(false);
		getItButton.interactable = false;
		
		coinIncreaseCount = 100;
	}

	private void EnableSkipButton()
	{
		skipButton.gameObject.SetActive(true);
	}
	
	private void Initialise()
	{
		_currentSkinPercentageUnlocked = PlayerPrefs.GetFloat("currentSkinPercentageUnlocked", 0f);

		if(ShopStateController.CurrentState.AreAllWeaponsUnlocked()) return;

		coloredWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderWeapon());
		blackWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderWeapon(), true);
		
		if((int)(_currentSkinPercentageUnlocked * 100) >= 100)
			percentageUnlockedText.text = 100 + "%";
		else 
			percentageUnlockedText.text = (int)(_currentSkinPercentageUnlocked * 100) + "%";

		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
	}

	private void FindNewLoaderWeapon(int currentWeapon)
	{
		var changed = false;
		
		//find a weapon from current index to last
		for (var i = currentWeapon + 1; i < MainShopController.GetWeaponSkinCount(); i++)
		{
			if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] != ShopItemState.Locked)
				continue;

			ShopStateController.CurrentState.SetNewLoaderWeapon(i);
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

				ShopStateController.CurrentState.SetNewLoaderWeapon(i);
				changed = true;
				break;
			}
		}

		//if still didn't find anything make sure loader isn't called anymore
		if (!changed)
			ShopStateController.CurrentState.AllWeaponsHaveBeenUnlocked();
		
		ResetLoader();
	}
	
	public void Skip()
	{
		if ((int) (_currentSkinPercentageUnlocked * 100) >= 100)
		{
			FindNewLoaderWeapon(GetLoaderWeapon());
			ResetLoader();
		}
		getItButton.interactable = false;
		skipButton.interactable = false;
		claimButton.interactable = false;
		FindObjectOfType<SidebarShopController>().OnGameEnd();
		
		DOVirtual.DelayedCall(3f, _mainCanvas.NextLevel);
	}

	public void GetIt() // Get it for WeaponLoader
	{
		if(!ApplovinManager.instance) return;
		if(!ApplovinManager.instance.TryShowRewardedAds()) return;
		
		StartWaiting(AdRewardType.NewWeapon);
		AdsMediator.StartListeningForAds(this);
	}
	
	public void Claim() // Claim for coin multiplier
	{
		if(!ApplovinManager.instance) return;
		if(!ApplovinManager.instance.TryShowRewardedAds()) return;

		StartWaiting(AdRewardType.CoinMultiplier);
		AdsMediator.StartListeningForAds(this);
	}
	
	private void ResetLoader()
	{
		_currentSkinPercentageUnlocked = 0f;
		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
		
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
	}
	
	private void ShowPanel()
	{
		InputHandler.AssignTemporaryDisabledState();

		blackBackground.gameObject.SetActive(true);
		var color = blackBackground.color;
		blackBackground.color = Color.clear;
		blackBackground.DOColor(color, .75f);
		
		var seq = DOTween.Sequence();

		seq.AppendInterval(LevelFlowController.only.isGiantLevel ? 2f : 1f);
		seq.AppendCallback(() => loaderPanel.SetActive(true));
		
		// show multiplier
		seq.AppendCallback(() =>
			barPivot.DOLocalRotate(new Vector3(0, 0, -90f), 0.65f)
			.SetEase(Ease.Flash)
			.SetLoops(-1, LoopType.Yoyo)
			.OnUpdate(() => claimMulTxt.text = GetMultiplierResult() * coinIncreaseCount + ""));

		if (!ShopStateController.CurrentState.AreAllWeaponsUnlocked())
		{
			//show loader
			var oldValue = _currentSkinPercentageUnlocked;
			_currentSkinPercentageUnlocked += 1 / (float) levelsPerUnlock;

			PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);

			seq.Append(DOTween.To(() => blackWeaponImage.fillAmount, value => blackWeaponImage.fillAmount = value,
				1 - _currentSkinPercentageUnlocked, tweenDuration).SetEase(Ease.OutBack));

			seq.Insert(1f,
				DOTween.To(() => oldValue, value => oldValue = value, _currentSkinPercentageUnlocked, tweenDuration)
					.SetEase(Ease.OutBack).OnUpdate(() => percentageUnlockedText.text = (int) (oldValue * 100) + "%"));
		}
		else
			percentageUI.SetActive(false);

		seq.AppendCallback(() =>
		{
			//skipButton.interactable = true;
			getItButton.interactable = true;
		});

		seq.AppendInterval(skipButtonWait);
		seq.AppendCallback(EnableSkipButton);

		if (_currentSkinPercentageUnlocked < 0.99f)
		{
			getItButton.gameObject.SetActive(false);
			//getItButton.interactable = false;
			//unlockedButtonsHolder.SetActive(false);
			return;
		}
		unlockedButtonsHolder.SetActive(true);
		//confetti particle fx for complete loader
	}
	
	private float ReturnMultiplierZRotation()
	{
		if (barPivot.localEulerAngles.z >= 0 && barPivot.localEulerAngles.z<=90)
		{
			return barPivot.localEulerAngles.z;
		}

		return 360 - barPivot.localEulerAngles.z;
	}

	private int GetMultiplierResult()
	{
		var z = ReturnMultiplierZRotation();

		if (z <= 90 && z >= 70)
			return 3;
		if (z >= 20 && z <= 69)
			return 2;
		return 5;
	}

	public bool ShouldShowNextLevel()
	{
		/*
		//if all weapons are unlocked, then show it yet.
			// because loader won't be called and after loader completes, next level won't be showed, so we show it on our own
		//else
			// if the loader is full, don't show it so that user only has choice between claim and skip
		*/
		if (ShopStateController.CurrentState.AreAllWeaponsUnlocked())
			return true;
		
		//we return this value because this is called before show panel is called and hence its value isn't updated 
		//so 0.8f is the value when the bar is about to reach 100 %
		return _currentSkinPercentageUnlocked < 0.79f;
	}

	private void OnWeaponPurchase(int index, bool shouldDeductCoins)
	{
		if (index != GetLoaderWeapon()) return;
		
		//find new skin to be unlocking
		FindNewLoaderWeapon(GetLoaderWeapon());
		
		//Reset loader so that it doesn't look inconsistent with new weapon being loaded
		ResetLoader();
	}

	private void OnGameEnd()
	{
		DOVirtual.DelayedCall(panelOpenWait, ShowPanel);
	}

	private void ReceiveWeaponLoaderReward()
	{
		AdsMediator.StopListeningForAds(this);
		
		claimButton.interactable = false;
		skipButton.interactable = false;
		getItButton.interactable = false;
		
		GameEvents.Only.InvokeWeaponSelect(GetLoaderWeapon(), false);
		
		DOVirtual.DelayedCall(0.25f, _mainCanvas.NextLevel);
		
		FindNewLoaderWeapon(GetLoaderWeapon());
		ResetLoader();
		//_mainCanvas.NextLevel();
	}

	private void ReceiveCoinMultiplierReward()
	{
		DOTween.Kill(barPivot);
		
		AdsMediator.StopListeningForAds(this);
		
		skipButton.interactable = false;
		claimButton.interactable = false;
		SidebarShopController.AlterCoinCount(GetMultiplierResult() * coinIncreaseCount);
		FindObjectOfType<SidebarShopController>().CoinsGoingUpEffect();
		
		DOVirtual.DelayedCall(2f, _mainCanvas.NextLevel);
	}

	private void StartWaiting(AdRewardType newType)
	{
		_currentAdRewardType = newType;
	}

	private void StopWaiting()
	{
		_currentAdRewardType = AdRewardType.None;
	}

	private void AdRewardReceiveBehaviour()
	{
		switch (_currentAdRewardType)
		{
			case AdRewardType.None:
				break;
			case AdRewardType.CoinMultiplier:
				ReceiveCoinMultiplierReward();
				break;
			case AdRewardType.NewWeapon:
				ReceiveWeaponLoaderReward();
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