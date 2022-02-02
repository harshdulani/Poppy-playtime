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

	[Header("Coin Particle Effect"), SerializeField] private TextMeshProUGUI coinText;
	[SerializeField] private RectTransform coinHolder;
	[SerializeField] private ParticleControlScript coinParticles;
	[SerializeField] private int coinIncreaseCount;

	private Animation _anim;
	private int _currentSpeedLevel, _currentPowerLevel, _currentSkin;
	
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
		MainShopController.shop.currentState.coinCount += change;
		MainShopController.shop.SaveCurrentShopState();
		UpdateCoinText();
	}
	
	private void AlterWeaponState(int relevantSkinIndex, ShopItemState newState)
	{
		MainShopController.shop.currentState.weaponStates[(WeaponType) relevantSkinIndex] = newState;
		MainShopController.shop.ChangeSelectedWeapon(relevantSkinIndex);
	}

	private static int GetCoinCount() => MainShopController.shop.currentState.coinCount;

	private void Start()
	{
		_anim = GetComponent<Animation>();
		
		Initialise();
		UpdateCoinText();
		
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
		_currentSkin = PlayerPrefs.GetInt("currentSkinInUse", 0);
		_currentPowerLevel = PlayerPrefs.GetInt("currentPowerLevel", 0);
		_currentSpeedLevel = PlayerPrefs.GetInt("currentSpeedLevel", 0);
	}

	private void UpdateButtons()
	{
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
		
		if(_currentSkin < MainShopController.GetSkinCount() - 1)
		{
			skinName.text = SkinLoader.GetSkinName(_currentSkin + 1).ToString();
			skinCostText.text = MainShopController.shop.weaponSkinCosts[_currentSkin + 1].ToString();
			skinButton.interactable = GetCoinCount() >= MainShopController.shop.weaponSkinCosts[_currentSkin + 1];
		}
		else
		{
			skinName.text = "MAX";
			skinCostText.text = "MAX";
			skinButton.interactable = false;
		}
		skinHand.SetActive(skinButton.interactable);
		
		currentSkinImage.sprite = SkinLoader.only.GetSkinSprite(_currentSkin + 1); //might be error prone
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
		AlterCoinCount(-MainShopController.shop.weaponSkinCosts[++_currentSkin]);
		AlterWeaponState(_currentSkin, ShopItemState.Selected);
		SkinLoader.only.UpdateSkinInUse(_currentSkin);
		InputHandler.Only.GetRightHand().UpdateEquippedSkin(false);
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	private void UpdateCoinText() => coinText.text = GetCoinCount().ToString();

	private void OnTapToPlay()
	{
		_anim.Play();
	}
	
	private void OnGameEnd()
	{
		var seq = DOTween.Sequence();
		seq.AppendInterval(1.25f);
		
		var initSize = coinText.fontSize;
		
		AudioManager.instance.Play("CoinCollect");
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize * 1.2f, .5f).SetEase(Ease.OutQuart));
		seq.Insert(1.5f, DOTween.To(GetCoinCount, AlterCoinCount, GetCoinCount() + coinIncreaseCount, 3f).SetEase(Ease.OutQuart));
		seq.InsertCallback(1.5f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder,false,false,false));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() =>
		{
			AlterCoinCount(coinIncreaseCount);
		});
		seq.AppendCallback(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>()
			.EnableNextLevel);
	}
}