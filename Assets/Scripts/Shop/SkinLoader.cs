using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinLoader : MonoBehaviour
{
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	
	[SerializeField] private Button  skipButton, claimButton;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText;

	[SerializeField] private GameObject loaderPanel, unlockedButtonsHolder; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait;

	private MainCanvasController _mainCanvas;
	private float _currentSkinPercentageUnlocked;

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
		skipButton.interactable = false;
		claimButton.interactable = false;
	}
	
	private void Initialise()
	{
		_currentSkinPercentageUnlocked = PlayerPrefs.GetFloat("currentSkinPercentageUnlocked", 0f);

		if(ShopStateController.CurrentState.AreAllWeaponsUnlocked()) return;

		coloredWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderWeapon(), false);
		blackWeaponImage.sprite = MainShopController.Main.GetWeaponSprite(GetLoaderWeapon(), true);
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
		FindNewLoaderWeapon(GetLoaderWeapon());
		ResetLoader();

		_mainCanvas.NextLevel();
	}

	public void Claim()
	{
		GameEvents.only.InvokeWeaponSelect(GetLoaderWeapon(), false);
		
		_mainCanvas.NextLevel();
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
			skipButton.interactable = true;
			claimButton.interactable = true;
		});

		if (_currentSkinPercentageUnlocked < 0.99f)
		{
			unlockedButtonsHolder.SetActive(false);
			return;
		}
		unlockedButtonsHolder.SetActive(true);
		//confetti particle fx for complete loader
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
	}
}