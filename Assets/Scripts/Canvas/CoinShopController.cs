using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinShopController : MonoBehaviour
{
	[SerializeField] private int[] speedLevelCosts, powerLevelCosts;
	[SerializeField] private Button speedButton, powerButton, skinButton;
	[SerializeField] private GameObject speedHand, powerHand, skinHand;
	[SerializeField] private Image currentSkinImage;
	[SerializeField] private TextMeshProUGUI speedMultiplier, speedCostText, powerMultiplier, powerCostText, skinName, skinCostText;
	[SerializeField] private Animation speedButtonPressAnimation, powerButtonPressAnimation, skinButtonPressAnimation;

	[Header("Coin Particle Effect"), SerializeField] private RectTransform coinHolder;
	[SerializeField] private ParticleControlScript coinParticles;
	[SerializeField] private int coinIncreaseCount;

	private Animation _anim;
	private int _currentSpeedLevel, _currentPowerLevel, _currentSkin, _currentSkinBeingUnlocked;
	
	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void AlterCoinCount(int change)
	{
		ShopReferences.refs.mainShop.currentState.coinCount += change;
		ShopReferences.refs.mainShop.SaveCurrentShopState();
		ShopReferences.refs.mainShop.UpdateCoinText();
	}
	
	private void AlterWeaponState(int relevantSkinIndex, ShopItemState newState)
	{
		ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) relevantSkinIndex] = newState;
		ShopReferences.refs.mainShop.ChangeSelectedWeapon(relevantSkinIndex);
	}

	private static int GetCoinCount() => ShopReferences.refs.mainShop.currentState.coinCount;

	private void Start()
	{
		_anim = GetComponent<Animation>();
		
		Initialise();
		ShopReferences.refs.mainShop.UpdateCoinText();
		
		UpdateButtons();
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.O)) return;

		AlterCoinCount(500);
		UpdateButtons();
	}

	private void Initialise()
	{
		_currentSkin = PlayerPrefs.GetInt("currentWeaponSkinInUse", 0);
		_currentSkinBeingUnlocked = PlayerPrefs.GetInt("currentSkinBeingUnlockedFromSideBar", 1);

		if (ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
			ShopItemState.Locked)
		{
			_currentSkinBeingUnlocked = (int) ShopReferences.refs.mainShop.currentState.weaponStates.First(state => state.Value == ShopItemState.Locked).Key;
			PlayerPrefs.SetInt("currentSkinBeingUnlockedFromSideBar", _currentSkinBeingUnlocked);
		}
		
		_currentPowerLevel = PlayerPrefs.GetInt("currentPowerLevel", 0);
		_currentSpeedLevel = PlayerPrefs.GetInt("currentSpeedLevel", 0);
	}

	public void UpdateButtons()
	{
		Initialise();
		//update texts and icons
		if(_currentSpeedLevel < speedLevelCosts.Length - 1)
		{
			speedMultiplier.text = "Speed: x" + (_currentSpeedLevel + 1);
			speedCostText.text = speedLevelCosts[_currentSpeedLevel + 1].ToString();
			speedButton.interactable = GetCoinCount() >= speedLevelCosts[_currentSpeedLevel + 1];
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
		}
		else
		{
			powerCostText.text = "MAX";
			powerMultiplier.text = "MAX";
			powerButton.interactable = false;
		}
		powerHand.SetActive(powerButton.interactable);
		
		if(_currentSkin < MainShopController.GetWeaponSkinCount() - 1)
		{
			while (ShopReferences.refs.mainShop.currentState.weaponStates[(WeaponType) _currentSkinBeingUnlocked] !=
				   ShopItemState.Locked)
			{
				_currentSkinBeingUnlocked++;
			}
			skinName.text = SkinLoader.GetWeaponSkinName(_currentSkinBeingUnlocked).ToString();
			skinCostText.text = ShopReferences.refs.mainShop.weaponSkinCosts[_currentSkinBeingUnlocked].ToString();
			skinButton.interactable = GetCoinCount() >= ShopReferences.refs.mainShop.weaponSkinCosts[_currentSkinBeingUnlocked];
		}
		else
		{
			skinName.text = "MAX";
			skinCostText.text = "MAX";
			skinButton.interactable = false;
		}
		skinHand.SetActive(skinButton.interactable);
		
		currentSkinImage.sprite = ShopReferences.refs.skinLoader.GetWeaponSkinSprite(_currentSkin + 1); //might be error prone
	}

	public void BuySpeed()
	{
		speedButtonPressAnimation.Play();
		AlterCoinCount(-speedLevelCosts[++_currentSpeedLevel]);
		InputHandler.Only.GetLeftHand().UpdatePullingSpeed(_currentSpeedLevel);
		PlayerPrefs.SetInt("currentSpeedLevel", _currentSpeedLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void BuyPower()
	{
		powerButtonPressAnimation.Play();
		AlterCoinCount(-powerLevelCosts[++_currentPowerLevel]);
		//ABSOLUTELY NO change in power script
		PlayerPrefs.SetInt("currentPowerLevel", _currentPowerLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void BuyNewSkin()
	{
		skinButtonPressAnimation.Play();
		AlterCoinCount(-ShopReferences.refs.mainShop.weaponSkinCosts[++_currentSkin]);
		_currentSkinBeingUnlocked++;
		PlayerPrefs.SetInt("currentSkinBeingUnlockedFromSideBar", _currentSkinBeingUnlocked);

		AlterWeaponState(_currentSkin, ShopItemState.Selected);
		ShopReferences.refs.skinLoader.UpdateWeaponSkinInUse(_currentSkin);
		InputHandler.Only.GetRightHand().UpdateEquippedWeaponsSkin(false);
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	private void OnTapToPlay()
	{
		_anim.Play();
	}
	
	private void OnGameEnd()
	{
		print("increasing");
		var seq = DOTween.Sequence();
		seq.AppendInterval(1.25f);

		var coinText = ShopReferences.refs.mainShop.GetCoinText();
		var initSize = ShopReferences.refs.mainShop.GetCoinText().fontSize;
		
		AudioManager.instance.Play("CoinCollect");
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize * 1.2f, .5f).SetEase(Ease.OutQuart));
		seq.InsertCallback(1.5f, () => AlterCoinCount(coinIncreaseCount));
		seq.InsertCallback(1.5f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder,false,false,false));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() =>
		{
			AlterCoinCount(coinIncreaseCount);
		});
		seq.AppendInterval(1.5f);
		seq.AppendCallback(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>()
			.EnableNextLevel);
	}
}