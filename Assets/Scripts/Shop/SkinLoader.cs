using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinLoader : MonoBehaviour
{
	[SerializeField] private RectTransform barPivot;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	
	[SerializeField] private Button skipButton,  getItButton, claimButton;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText, claimMulTxt;

	[SerializeField] private GameObject loaderPanel, unlockedButtonsHolder; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait;

	[Header("Coin Particle Effect"), SerializeField] private RectTransform coinHolder;
	[SerializeField] private ParticleControlScript coinParticles;
	[SerializeField] private int coinIncreaseCount = 100;
	
	private MainCanvasController _mainCanvas;
	private float _currentSkinPercentageUnlocked;

	public bool _getItAdsWeaponLoader, _claimAdsMulXCoins;
	private bool _shouldWaitForAds;
	
	private static int GetLoaderWeapon() => ShopStateController.CurrentState.GetState().LoaderWeapon;

	private void OnEnable()
	{
		GameEvents.only.weaponSelect += OnWeaponPurchase;
		
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.weaponSelect -= OnWeaponPurchase;
		
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void Start()
	{
		_mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>();
		
		Initialise();
		
		loaderPanel.SetActive(false);
		//skipButton.interactable = false;
		getItButton.interactable = false;

		barPivot.DOLocalRotate(new Vector3(0, 0, -90f), 0.65f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
		Invoke(nameof(EnableSkipButton),10);
		coinIncreaseCount = 100;
	}
	
	private void Update()
	{
		claimMulTxt.text = (GetMultiplierResult() * coinIncreaseCount)+ "";
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
			//SidebarShopController.AlterCoinCount(100);
		}
		getItButton.interactable = false;
		skipButton.interactable = false;
		claimButton.interactable = false;
		FindObjectOfType<SidebarShopController>().OnGameEnd();
		_mainCanvas.Invoke(nameof(MainCanvasController.NextLevel),3f);
	}

	public void GetIt() // Get it for WeaponLoader
	{
		if(!ApplovinManager.instance)
			return;

		_getItAdsWeaponLoader = true;
		_shouldWaitForAds = true;
		ApplovinManager.instance.ShowRewardedAds();
		
		
	}
	
	public void Claim()
	{
		if(!ApplovinManager.instance)
			return;

		_claimAdsMulXCoins = true;
		ApplovinManager.instance.ShowRewardedAds();
	}
	
	private void ResetLoader()
	{
		_currentSkinPercentageUnlocked = 0f;
		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
		
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
	}
	
	private void ShowPanel()
	{
		InputHandler.Only.AssignDisabledState();
		loaderPanel.SetActive(true);
		
		var oldValue = _currentSkinPercentageUnlocked;
		_currentSkinPercentageUnlocked += 1 / (float)levelsPerUnlock;
		
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);

		var seq = DOTween.Sequence();

		seq.AppendInterval(1f);
		seq.Append(DOTween.To(() => blackWeaponImage.fillAmount, value => blackWeaponImage.fillAmount = value,
			1 - _currentSkinPercentageUnlocked, tweenDuration).SetEase(Ease.OutBack));
		
		seq.Insert(1f,
			DOTween.To(() => oldValue, value => oldValue = value, _currentSkinPercentageUnlocked, tweenDuration)
				.SetEase(Ease.OutBack).OnUpdate(() => percentageUnlockedText.text = (int) (oldValue * 100) + "%"));
		
		seq.AppendCallback(() =>
		{
			//skipButton.interactable = true;
			getItButton.interactable = true;
		});

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
		if(ShopStateController.CurrentState.AreAllWeaponsUnlocked()) return;
		
		Invoke(nameof(ShowPanel), panelOpenWait);
		
		// var seq = DOTween.Sequence();
		// seq.AppendInterval(1.25f);
		//
		// var coinText = MainShopController.Main.GetCoinText();
		// var initSize = MainShopController.Main.GetCoinText().fontSize;
		//
		// var dummyCoinCount = SidebarShopController.GetCoinCount();
		// AudioManager.instance.Play("CoinCollect");
	}
	
	public void Callback_WeaponLoader()
	{
		_getItAdsWeaponLoader = false;
		
		claimButton.interactable = false;
		skipButton.interactable = false;
		getItButton.interactable = false;
		
		GameEvents.only.InvokeWeaponSelect(GetLoaderWeapon(), false);
		_mainCanvas.Invoke(nameof(MainCanvasController.NextLevel),0.25f);
		FindNewLoaderWeapon(GetLoaderWeapon());
		ResetLoader();
		//_mainCanvas.NextLevel();
	}

	public void Callback_MulXCoins()
	{
		DOTween.Kill(barPivot);
		_claimAdsMulXCoins = false;
		skipButton.interactable = false;
		claimButton.interactable = false;
		SidebarShopController.AlterCoinCount(GetMultiplierResult() * coinIncreaseCount);
		FindObjectOfType<SidebarShopController>().CoinsGoingUpEffect();
		_mainCanvas.Invoke(nameof(MainCanvasController.NextLevel),2f);
	}
}