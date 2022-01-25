using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinShopController : MonoBehaviour
{
	[SerializeField] private int[] speedLevelCosts, powerLevelCosts, skinCosts;
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
	private int _coinCount = 0;

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
	
	public int CoinCount
	{
		get => _coinCount;
		private set { _coinCount = value;
			UpdateCoinText();
		}
	}

	private void Start()
	{
		_anim = GetComponent<Animation>();
		
		Initialise();
		UpdateCoinText();
		
		UpdateButtons();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			CoinCount += 500;
			UpdateCoinAmount();
			UpdateButtons();
		}
	}

	private void Initialise()
	{
		CoinCount = PlayerPrefs.GetInt("coinCount", 0);
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
			speedButton.interactable = CoinCount >= speedLevelCosts[_currentSpeedLevel + 1];
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
			powerButton.interactable = CoinCount >= powerLevelCosts[_currentPowerLevel + 1];
		}
		else
		{
			powerCostText.text = "MAX";
			powerMultiplier.text = "MAX";
			powerButton.interactable = false;
		}
		powerHand.SetActive(powerButton.interactable);
		
		if(_currentSkin < skinCosts.Length - 1)
		{
			skinName.text = SkinLoader.GetSkinName(_currentSkin + 1).ToString();
			skinCostText.text = skinCosts[_currentSkin + 1].ToString();
			skinButton.interactable = CoinCount >= skinCosts[_currentSkin + 1];
		}
		else
		{
			skinName.text = "MAX";
			skinCostText.text = "MAX";
			skinButton.interactable = false;
		}
		skinHand.SetActive(skinButton.interactable);
		
		currentSkinImage.sprite = transform.parent.GetComponentInChildren<SkinLoader>().GetSkinSprite(_currentSkin + 1); //might be error prone
	}

	public void BuySpeed()
	{
		speedButtonPressAnimation.Play();
		CoinCount -= speedLevelCosts[++_currentSpeedLevel];
		InputHandler.Only.GetLeftHand().UpdatePullingSpeed(_currentSpeedLevel);
		PlayerPrefs.SetInt("currentSpeedLevel", _currentSpeedLevel);
		UpdateCoinAmount();

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void BuyPower()
	{
		powerButtonPressAnimation.Play();
		CoinCount -= powerLevelCosts[++_currentPowerLevel];
		//ABSOLUTELY NO change in power script
		PlayerPrefs.SetInt("currentPowerLevel", _currentPowerLevel);
		UpdateCoinAmount();
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void BuyNewSkin()
	{
		skinButtonPressAnimation.Play();
		CoinCount -= skinCosts[++_currentSkin];
		SkinLoader.only.UpdateSkinInUse(_currentSkin);
		InputHandler.Only.GetRightHand().UpdateEquippedSkin(false);
		UpdateCoinAmount();
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}
	
	private void UpdateCoinText() => coinText.text = CoinCount.ToString();

	private void UpdateCoinAmount() => PlayerPrefs.SetInt("coinCount", CoinCount);

	private void OnTapToPlay()
	{
		_anim.Play();
	}
	
	private void OnGameEnd()
	{
		var seq = DOTween.Sequence();
		seq.AppendInterval(1.25f);
		
		var initSize = coinText.fontSize;
		CoinCount += coinIncreaseCount;
		UpdateCoinAmount();
		
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize * 1.2f, .5f).SetEase(Ease.OutQuart));
		seq.Insert(1.5f, DOTween.To(() => _coinCount, value => _coinCount = value, _coinCount + coinIncreaseCount, 3f).SetEase(Ease.OutQuart).OnUpdate(UpdateCoinText));
		seq.InsertCallback(1.5f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder,false,false,false));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() =>
		{
			AudioManager.instance.Play("CoinCollect");
			_coinCount += coinIncreaseCount;
		});
		seq.AppendCallback(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>()
			.EnableNextLevel);
	}
}