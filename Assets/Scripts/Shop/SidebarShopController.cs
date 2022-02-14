using DG.Tweening;
using Dreamteck.Splines.Primitives;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SidebarShopController : MonoBehaviour
{
	public bool speedAdsBtnClicked, powerAdsBtnClicked, skinAdsBtnClicked;
	
	[SerializeField] private Sprite VideoBtn, normalBtn;
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
	private int _currentSpeedLevel, _currentPowerLevel;
	
	private static int GetSidebarWeapon() => ShopStateController.CurrentState.GetState().SidebarWeapon;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		
		GameEvents.only.weaponSelect += OnWeaponPurchase;
		
		//GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		
		GameEvents.only.weaponSelect -= OnWeaponPurchase;
		
	//	GameEvents.only.gameEnd -= OnGameEnd;
	}

	public static void AlterCoinCount(int change)
	{
		ShopStateController.CurrentState.GetState().CoinCount += change;
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.UpdateCoinText();
		MainShopController.Main.ReadCurrentShopState();
	}

	public static int GetCoinCount() => ShopStateController.CurrentState.GetState().CoinCount;

	private void Start()
	{
		_anim = GetComponent<Animation>();
		
		_currentPowerLevel = PlayerPrefs.GetInt("currentPowerLevel", 0);
		_currentSpeedLevel = PlayerPrefs.GetInt("currentSpeedLevel", 0);
		
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
			//speedButton.interactable = GetCoinCount() >= speedLevelCosts[_currentSpeedLevel + 1];
			if (GetCoinCount() < speedLevelCosts[_currentSpeedLevel + 1])
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
			speedCostText.text = "MAX";
			speedMultiplier.text = "MAX";
			speedButton.interactable = false;
		}
		speedHand.SetActive(speedButton.interactable);
		
		if(_currentPowerLevel < powerLevelCosts.Length - 1)
		{
			powerMultiplier.text = "Power: x" + (_currentPowerLevel + 1);
			powerCostText.text = powerLevelCosts[_currentPowerLevel + 1].ToString();
			//powerButton.interactable = GetCoinCount() >= powerLevelCosts[_currentPowerLevel + 1];
			if (GetCoinCount() < powerLevelCosts[_currentSpeedLevel + 1])
			{
				powerButton.image.sprite = VideoBtn;
				powerCostText.gameObject.SetActive(false);
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
			//skinButton.interactable = GetCoinCount() >= MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()];
			if (GetCoinCount() < MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()])
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
		
		skinHand.SetActive(skinButton.interactable);
		
		currentSkinImage.sprite = MainShopController.Main.GetWeaponSprite(GetSidebarWeapon());
	}

	public void BuySpeed()
	{
		if (GetCoinCount() < speedLevelCosts[_currentSpeedLevel + 1])
		{
			if(!ApplovinManager.instance)
				return;

			print("sss");
			speedAdsBtnClicked = true;
			ApplovinManager.instance.ShowRewardedAds();
		}
		else
		{
			SpeedWithCoins();
		}
		//confetti and/or power up vfx
	}

	public void SpeedWithCoins()
	{
		speedAdsBtnClicked = false;
		speedButtonPressAnimation.Play();
		
		AlterCoinCount(-speedLevelCosts[++_currentSpeedLevel]);
		InputHandler.Only.GetLeftHand().UpdatePullingSpeed(_currentSpeedLevel);
		PlayerPrefs.SetInt("currentSpeedLevel", _currentSpeedLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
	}

	public void Callback_Speed()
	{
		speedAdsBtnClicked = false;
		speedButtonPressAnimation.Play();
		
		AlterCoinCount(speedLevelCosts[++_currentSpeedLevel]);
		AlterCoinCount(-speedLevelCosts[++_currentSpeedLevel]);
		InputHandler.Only.GetLeftHand().UpdatePullingSpeed(_currentSpeedLevel);
		PlayerPrefs.SetInt("currentSpeedLevel", _currentSpeedLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
	}

	public void BuyPower()
	{

		if (GetCoinCount() < powerLevelCosts[_currentSpeedLevel + 1])
		{
			if(!ApplovinManager.instance)
				return;

			powerAdsBtnClicked = true;
			ApplovinManager.instance.ShowRewardedAds();
		}
		else
		{
			PowerWithCoins();
		}

	}

	public void PowerWithCoins()
	{
		powerAdsBtnClicked = false;
		powerButtonPressAnimation.Play();
		AlterCoinCount(-powerLevelCosts[++_currentPowerLevel]);
		//ABSOLUTELY NO change in power script
		PlayerPrefs.SetInt("currentPowerLevel", _currentPowerLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void Callback_Power()
	{
		powerAdsBtnClicked = false;
		powerButtonPressAnimation.Play();
		AlterCoinCount(powerLevelCosts[++_currentPowerLevel]);
		AlterCoinCount(-powerLevelCosts[++_currentPowerLevel]);
		//ABSOLUTELY NO change in power script
		PlayerPrefs.SetInt("currentPowerLevel", _currentPowerLevel);

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	public void BuyNewWeapon()
	{
		if (GetCoinCount() < MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()])
		{
			if(!ApplovinManager.instance)
				return;

			skinAdsBtnClicked = true;
			ApplovinManager.instance.ShowRewardedAds();
		}
		else
		{
			NewWeapon_WithWeapons();
			//confetti and/or power up vfx
		}
	}

	void NewWeapon_WithWeapons()
	{
		skinAdsBtnClicked = false;
		skinButtonPressAnimation.Play();
		GameEvents.only.InvokeWeaponSelect(GetSidebarWeapon(), true);
		AudioManager.instance.Play("Button");
	}


	public void Callback_NewWeapon()
	{
		skinAdsBtnClicked = false;
		skinButtonPressAnimation.Play();
		AlterCoinCount(MainShopController.Main.weaponSkinCosts[GetSidebarWeapon()]);
		GameEvents.only.InvokeWeaponSelect(GetSidebarWeapon(), true);
		AudioManager.instance.Play("Button");
	}
	

	private void OnWeaponPurchase(int index, bool shouldDeductCoins)
	{
		if(index != GetSidebarWeapon()) return;
		
		FindNewSideBarWeapon(GetSidebarWeapon());

		DOVirtual.DelayedCall(0.25f, UpdateButtons);
	}
	
	private void OnTapToPlay()
	{
		_anim.Play();
	}
	
	public void OnGameEnd()
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
		seq.InsertCallback(.75f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder,false,false,false));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() =>
		{
			print("sssssssss");
			AlterCoinCount(coinIncreaseCount);
		});
		seq.AppendInterval(1.5f);
		seq.AppendCallback(GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>()
			.EnableNextLevel);
	}

	public void CoinsGoingUpEffect()
	{
		coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder, false, false, false);
	}
}