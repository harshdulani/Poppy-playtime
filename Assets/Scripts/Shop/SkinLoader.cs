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
		Loader
	}

	[SerializeField] private Image blackBackground;

	[Header("Money Panel"), SerializeField]
	private GameObject moneyPanel;

	[SerializeField] private Transform barPivot, moneyRayBeams;
	[SerializeField] private Button claimMoneyButton, skipMoneyButton;
	[SerializeField] private TextMeshProUGUI moneyButtonText, headingText;

	[Header("Skin Panel"), SerializeField] 
	private GameObject skinPanel;
	[SerializeField] private Transform skinRayBeams;
	[SerializeField] private Button claimSkinButton, skipSkinButton;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText, skinSkipButtonText;
	[SerializeField] private bool unlockWeapons;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	
	[Header("Bonus Level Panel"), SerializeField]
	private GameObject bonusLevelPanel;

	[Header("Settings"), SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait;

	[SerializeField] private int coinIncreaseCount = 100;

	private MainCanvasController _mainCanvas;
	private float _currentSkinPercentageUnlocked;
	private bool _hasGameEnded;

	private AdRewardType _currentAdRewardType;

	private static int GetLoaderIndex() => ShopStateController.CurrentState.GetState().LoaderIndex;

	private void OnEnable()
	{
		if (unlockWeapons)
			GameEvents.Only.WeaponSelect += OnWeaponPurchase;
		else
			GameEvents.Only.SkinSelect += OnSkinSelect;

		GameEvents.Only.GameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		if(unlockWeapons)
			GameEvents.Only.WeaponSelect -= OnWeaponPurchase;
		else
			GameEvents.Only.SkinSelect -= OnSkinSelect;

		GameEvents.Only.GameEnd -= OnGameEnd;
	}

	private void OnDestroy() => AdsMediator.StopListeningForAds(this);

	private void Start()
	{
		_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>();

		Initialise();
		
		var levelNo = PlayerPrefs.GetInt("levelNo", 1);
		headingText.text = "Level " + levelNo + " Completed!";
		
		skipMoneyButton.gameObject.SetActive(false);
		skipSkinButton.gameObject.SetActive(false);
		claimSkinButton.interactable = false;
	}

	private void Initialise()
	{
		_currentSkinPercentageUnlocked = PlayerPrefs.GetFloat("currentSkinPercentageUnlocked", 0f);

		if (unlockWeapons)
		{
			if (ShopStateController.CurrentState.AreAllWeaponsUnlocked()) return;
		
			coloredWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderIndex());
			blackWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderIndex(), true);
		}
		else
		{
			if (ShopStateController.CurrentState.AreAllArmSkinsUnlocked()) return;

			coloredWeaponImage.sprite = MainShopController.Main.GetArmsSkinSprite(GetLoaderIndex());
			blackWeaponImage.sprite = MainShopController.Main.GetArmsSkinSprite(GetLoaderIndex(), true);
		}

		if ((int) (_currentSkinPercentageUnlocked * 100) >= 100)
			percentageUnlockedText.text = 100 + "%";
		else
			percentageUnlockedText.text = (int) (_currentSkinPercentageUnlocked * 100) + "%";

		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
	}
	
	private void ShowMoneyPanel()
	{
		InputHandler.AssignTemporaryDisabledState();

		blackBackground.gameObject.SetActive(true);
		var color = blackBackground.color;
		blackBackground.color = Color.clear;
		blackBackground.DOColor(color, .75f);

		moneyRayBeams.DORotate(Vector3.forward * 180, 4f)
			.SetLoops(-1, LoopType.Incremental)
			.SetEase(Ease.Linear);

		barPivot.DOLocalRotate(new Vector3(0, 0, -90f), 0.65f)
			.SetDelay(LevelFlowController.only.isGiantLevel ? 2f : 1f)
			.OnStart(() => moneyPanel.SetActive(true))
			.SetEase(Ease.Flash)
			.SetLoops(-1, LoopType.Yoyo)
			.OnUpdate(() => moneyButtonText.text = "Claim " + GetMultiplierResult() * coinIncreaseCount + "");
		
		DOVirtual.DelayedCall(panelOpenWait, EnableSkipMoneyButton);
	}

	public void ClaimMoneyOnButton() // Claim for coin multiplier
	{
		claimMoneyButton.interactable = false;
		
		if (!ApplovinManager.instance) return;
		if (!ApplovinManager.instance.TryShowRewardedAds()) return;
	
		skipMoneyButton.interactable = false;
		StartWaiting(AdRewardType.CoinMultiplier);
		DOTween.Kill(barPivot);

		AdsMediator.StartListeningForAds(this);
	}

	public void SkipMoneyOnButton()
	{
		SidebarShopController.only.IncreaseCoinsBy(coinIncreaseCount);
		
		MoneyPanelButtonBehaviour();
	}

	private void MoneyPanelButtonBehaviour()
	{
		skipMoneyButton.interactable = false;
		claimMoneyButton.interactable = false;

		DOVirtual.DelayedCall(2f, () =>
		{
			moneyPanel.SetActive(false);
			//if everything is not already unlocked, show skin panel or directly skip to bonus level if there exists one
			if (!(unlockWeapons ?
				ShopStateController.CurrentState.AreAllWeaponsUnlocked() :
				ShopStateController.CurrentState.AreAllArmSkinsUnlocked()))
				ShowSkinPanel();
			else if (bonusLevelPanel)
				ShowBonusLevelPanel();
		});
	}
	
	private void EnableSkipMoneyButton() => skipMoneyButton.gameObject.SetActive(true);
	
	private void ShowSkinPanel()
	{
		skinPanel.SetActive(true);
		
		var oldValue = _currentSkinPercentageUnlocked;
		_currentSkinPercentageUnlocked += 1 / (float) levelsPerUnlock;

		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);

		skinRayBeams.DORotate(Vector3.forward * 180, 4f)
			.SetLoops(-1, LoopType.Incremental)
			.SetEase(Ease.Linear);
		
		DOTween.To(() => blackWeaponImage.fillAmount, value => blackWeaponImage.fillAmount = value,
				1 - _currentSkinPercentageUnlocked, tweenDuration)
			.SetEase(Ease.OutBack);

		DOTween.To(() => oldValue, value => oldValue = value, _currentSkinPercentageUnlocked, tweenDuration)
			.SetEase(Ease.OutBack)
			.OnUpdate(() => percentageUnlockedText.text = (int) (oldValue * 100) + "%");

		claimSkinButton.interactable = true;

		DOVirtual.DelayedCall(panelOpenWait, EnableSkipSkinButton);

		if (_currentSkinPercentageUnlocked > 0.99f) return;
		
		claimSkinButton.gameObject.SetActive(false);
		skinSkipButtonText.text = "Continue";
	}

	public void ClaimSkinOnButton() // Get it for WeaponLoader
	{
		if (!ApplovinManager.instance) return;
		if (!ApplovinManager.instance.TryShowRewardedAds()) return;
		
		StartWaiting(AdRewardType.Loader);
		AdsMediator.StartListeningForAds(this);
	}

	public void SkipSkinOnButton()
	{
		if ((int) (_currentSkinPercentageUnlocked * 100) >= 100)
		{
			if(unlockWeapons)
				FindNewLoaderWeapon(GetLoaderIndex());
			else
				FindNewLoaderArmsSkin(GetLoaderIndex());
			
			ResetLoader();
		}

		claimSkinButton.interactable = false;
		skipMoneyButton.interactable = false;
		claimMoneyButton.interactable = false;

		if(bonusLevelPanel) return;
		DOVirtual.DelayedCall(0.1f, _mainCanvas.NextLevel);
	}

	private void EnableSkipSkinButton() => skipSkinButton.gameObject.SetActive(true);

	private void ShowBonusLevelPanel()
	{
		moneyPanel.SetActive(false);
		skinPanel.SetActive(false);
		bonusLevelPanel.SetActive(true);
		
		bonusLevelPanel.transform.GetChild(0).DORotate(Vector3.forward * 180, 4f)
			.SetLoops(-1, LoopType.Incremental)
			.SetEase(Ease.Linear);
	}

	public void SkipBonusOnButton()
	{
		DOVirtual.DelayedCall(0.1f, _mainCanvas.NextLevel);
	}
	
	private void ResetLoader()
	{
		_currentSkinPercentageUnlocked = 0f;
		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;

		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
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

	private float ReturnMultiplierZRotation()
	{
		if (barPivot.localEulerAngles.z >= 0 && barPivot.localEulerAngles.z<=90)
		{
			return barPivot.localEulerAngles.z;
		}

		return 360 - barPivot.localEulerAngles.z;
	}

	private void FindNewLoaderWeapon(int currentIndex)
	{
		var changed = false;

		//find a weapon from current index to last
		for (var i = currentIndex + 1; i < MainShopController.GetWeaponSkinCount(); i++)
		{
			if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] != ShopItemState.Locked)
				continue;

			ShopStateController.CurrentState.SetNewLoaderIndex(i);
			changed = true;
			break;
		}

		//if all weapons after me are unlocked, try to find new before me
		if (!changed)
		{
			for (var i = 1; i < currentIndex; i++)
			{
				if (ShopStateController.CurrentState.GetState().weaponStates[(WeaponType) i] != ShopItemState.Locked)
					continue;

				ShopStateController.CurrentState.SetNewLoaderIndex(i);
				changed = true;
				break;
			}
		}

		//if still didn't find anything make sure loader isn't called anymore
		if (!changed)
			ShopStateController.CurrentState.AllWeaponsHaveBeenUnlocked();

		ResetLoader();
	}

	private void FindNewLoaderArmsSkin(int currentIndex)
	{
		var changed = false;

		//find a weapon from current index to last
		for (var i = currentIndex + 1; i < MainShopController.GetArmsSkinCount(); i++)
		{
			if (ShopStateController.CurrentState.GetState().armStates[(ArmsType) i] != ShopItemState.Locked)
				continue;

			ShopStateController.CurrentState.SetNewLoaderIndex(i);
			changed = true;
			break;
		}

		//if all weapons after me are unlocked, try to find new before me
		if (!changed)
		{
			for (var i = 1; i < currentIndex; i++)
			{
				if (ShopStateController.CurrentState.GetState().armStates[(ArmsType) i] != ShopItemState.Locked)
					continue;

				ShopStateController.CurrentState.SetNewLoaderIndex(i);
				changed = true;
				break;
			}
		}

		//if still didn't find anything make sure loader isn't called anymore
		if (!changed)
			ShopStateController.CurrentState.AllWeaponsHaveBeenUnlocked();

		ResetLoader();
	}

	private void OnWeaponPurchase(int index, bool shouldDeductCoins)
	{
		if (index != GetLoaderIndex()) return;
		
		//find new skin to be unlocking
		FindNewLoaderWeapon(GetLoaderIndex());
		
		//Reset loader so that it doesn't look inconsistent with new weapon being loaded
		ResetLoader();
	}

	private void OnSkinSelect(int index, bool shouldDeductCoins)
	{
		if (index != GetLoaderIndex()) return;
		
		//find new skin to be unlocking
		FindNewLoaderArmsSkin(GetLoaderIndex());
		
		//Reset loader so that it doesn't look inconsistent with new weapon being loaded
		ResetLoader();
	}

	private void OnGameEnd()
	{
		if(_hasGameEnded) return;

		_hasGameEnded = true;
		DOVirtual.DelayedCall(panelOpenWait, () =>
		{
			AudioManager.instance.Play("Win");
			ShowMoneyPanel();
		});
	}

	private void ReceiveLoaderReward()
	{
		AdsMediator.StopListeningForAds(this);
		
		claimSkinButton.interactable = false;
		skipSkinButton.interactable = false;
		claimMoneyButton.interactable = false;
		
		GameEvents.Only.InvokeWeaponSelect(GetLoaderIndex(), false);
		
		DOVirtual.DelayedCall(0.25f, _mainCanvas.NextLevel);
		
		if(unlockWeapons)
			FindNewLoaderWeapon(GetLoaderIndex());
		else
			FindNewLoaderArmsSkin(GetLoaderIndex());
		ResetLoader();
	}

	private void ReceiveCoinMultiplierReward()
	{
		SidebarShopController.only.IncreaseCoinsBy(GetMultiplierResult() * coinIncreaseCount);
		AdsMediator.StopListeningForAds(this);

		MoneyPanelButtonBehaviour();
	}

	private void StartWaiting(AdRewardType newType) => _currentAdRewardType = newType;

	private void StopWaiting() => _currentAdRewardType = AdRewardType.None;

	private void AdRewardReceiveBehaviour()
	{
		switch (_currentAdRewardType)
		{
			case AdRewardType.None:
				break;
			case AdRewardType.CoinMultiplier:
				ReceiveCoinMultiplierReward();
				break;
			case AdRewardType.Loader:
				ReceiveLoaderReward();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		StopWaiting();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo) => AdRewardReceiveBehaviour();

	public void OnShowDummyAd() => AdRewardReceiveBehaviour();

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