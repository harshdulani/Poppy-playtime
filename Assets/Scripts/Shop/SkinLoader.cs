using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinLoader : MonoBehaviour
{
	[SerializeField] private Sprite[] coloredWeaponSprites, blackWeaponSprites;
	[SerializeField] private Sprite[] coloredArmSprites, blackArmSprites;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	
	[SerializeField] private Button  skipButton, claimButton;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText;

	[SerializeField] private GameObject loaderPanel, unlockedButtonsHolder; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait;

	private int _currentWeaponSkinInUse;
	private int  _currentSkinBeingUnlocked = 1;
	private float _currentSkinPercentageUnlocked;

	private void OnEnable()
	{
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void Start()
	{
		Initialise();
		
		loaderPanel.SetActive(false);
		skipButton.interactable = false;
		claimButton.interactable = false;
	}
	
	private void Initialise()
	{
		_currentSkinBeingUnlocked = PlayerPrefs.GetInt("currentSkinBeingUnlocked", 1);
		_currentSkinPercentageUnlocked = PlayerPrefs.GetFloat("currentSkinPercentageUnlocked", 0f);
		_currentWeaponSkinInUse = PlayerPrefs.GetInt("currentWeaponSkinInUse", 0);

		if(_currentSkinBeingUnlocked >= MainShopController.GetWeaponSkinCount()) return;
		
		coloredWeaponImage.sprite = coloredWeaponSprites[_currentSkinBeingUnlocked];
		blackWeaponImage.sprite = blackWeaponSprites[_currentSkinBeingUnlocked];
		percentageUnlockedText.text = (int)(_currentSkinPercentageUnlocked * 100) + "%";

		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
	}

	public Sprite GetWeaponSkinSprite(int index = -1, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackWeaponSprites : coloredWeaponSprites;
		
		if (index == -1)
			return currentList[PlayerPrefs.GetInt("currentWeaponSkinInUse", 0)];
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}

	public Sprite GetArmsSkinType(int index = -1, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackArmSprites : coloredArmSprites;
		
		if (index == -1)
			return currentList[PlayerPrefs.GetInt("currentArmsSkinInUse", 0)];
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}

	public static WeaponType GetWeaponSkinName(int index = -1) => (WeaponType) (index == -1 ? PlayerPrefs.GetInt("currentWeaponSkinInUse", 0) : index);
	
	public static ArmsType GetArmsSkinName(int index = -1) => (ArmsType) (index == -1 ? PlayerPrefs.GetInt("currentArmsSkinInUse", 0) : index);

	public void UpdateWeaponSkinInUse(int currentSkin)
	{
		_currentWeaponSkinInUse = currentSkin;
		PlayerPrefs.SetInt("currentWeaponSkinInUse", _currentWeaponSkinInUse);
		
		for (var i = 0; i < MainShopController.GetWeaponSkinCount(); i++)
		{
			if(ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
			   ShopItemState.Locked)
				continue;

			_currentSkinBeingUnlocked = i;
			PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
		}
		ShopReferences.refs.sidebar.UpdateButtons();
		ResetLoader();
	}
	
	public void UpdateArmsSkinInUse(int currentSkin)
	{
		PlayerPrefs.SetInt("currentArmsSkinInUse", currentSkin);
	}

	public void Skip()
	{
		loaderPanel.SetActive(false);
		
		//this value is being set in resetloader
		if(_currentSkinBeingUnlocked < MainShopController.GetWeaponSkinCount() - 1)
		{
			var changed = false;

			for (var i = _currentSkinBeingUnlocked + 1; i < MainShopController.GetWeaponSkinCount(); i++)
			{
				if(ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
					ShopItemState.Locked)
					continue;

				_currentSkinBeingUnlocked = i;
				PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
				changed = true;
			}
			if(!changed)
			{
				for (var i = 0; i < _currentSkinBeingUnlocked; i++)
				{
					if(ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
					   ShopItemState.Locked)
						continue;

					_currentSkinBeingUnlocked = i;
					PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
				}
			}
		}
		//play animatn
		ResetLoader();
		ShopReferences.refs.sidebar.UpdateButtons();

		GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>().NextLevel();
	}

	public void Claim()
	{
		if(_currentWeaponSkinInUse <= MainShopController.GetWeaponSkinCount() - 1)
		{
			_currentWeaponSkinInUse++;
			PlayerPrefs.SetInt("currentWeaponSkinInUse", _currentWeaponSkinInUse);
		}
		
		if(_currentSkinBeingUnlocked < MainShopController.GetWeaponSkinCount() - 1)
		{
			for (var i = 0; i < MainShopController.GetWeaponSkinCount(); i++)
			{
				if(ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
				   ShopItemState.Locked)
					continue;

				_currentSkinBeingUnlocked = i;
				PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
			}
		}
		
		_currentSkinPercentageUnlocked = 0f;
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
		//confetti
		
		loaderPanel.SetActive(false);

		GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>().NextLevel();
	}

	private void ResetLoader()
	{
		_currentSkinPercentageUnlocked = 0f;
		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
		
		PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
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
		if (_currentSkinBeingUnlocked == _currentWeaponSkinInUse)
			return false;
		
		//we return this value becuase this is called before show panel is called and hence its value isnt updated 
		//so 0.8f is the value when the bar is about to reach 100 %
		return _currentSkinPercentageUnlocked < 0.79f;
	}

	private void OnGameEnd()
	{
		if(_currentSkinBeingUnlocked >= MainShopController.GetWeaponSkinCount() - 1) return;
		
		Invoke(nameof(ShowPanel), panelOpenWait);
	}
}