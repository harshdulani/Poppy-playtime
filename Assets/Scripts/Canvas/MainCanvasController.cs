using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour, IWantsAds
{
	[SerializeField] private int lastRegularLevel;
	
	[SerializeField] private GameObject holdToAim, victory, defeat, nextLevel, retry, constantRetryButton, skipLevel;
	[SerializeField] private TextMeshProUGUI levelText, instructionText;
	[SerializeField] private Image red;
	[SerializeField] private Toggle abToggle;
	[SerializeField] private string tapInstruction, swipeInstruction;

	[SerializeField] private Button nextLevelButton;

	private LevelIndicator _indicator;
	private bool _hasTapped, _hasLost;

	private void OnEnable()
	{
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
		GameEvents.Only.GameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
		GameEvents.Only.GameEnd -= OnGameEnd;
	}

	private void Start()
	{
		_indicator = GetComponent<LevelIndicator>();

		var levelNo = PlayerPrefs.GetInt("levelNo", 1);
		levelText.text = "Level " + levelNo;
		abToggle.isOn = PlayerPrefs.GetInt("controlMechanic", 0) == 0;
		instructionText.text = abToggle.isOn ? tapInstruction : swipeInstruction;
		
		if(levelNo < 5)
		{
			skipLevel.SetActive(false);
			_indicator.SetIndicatorEnable(false);
		}

		PlayerPrefs.SetInt("lastRegularLevel", lastRegularLevel);
		
		if(GAScript.Instance)
			GAScript.Instance.LevelStart(PlayerPrefs.GetInt("levelNo", 0).ToString());
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.N)) NextLevel();
		
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(_hasTapped) return;
		
		if (!InputExtensions.GetFingerDown()) return;
		
		if(!EventSystem.current) { print("no event system"); return; }
		
		if(!EventSystem.current.IsPointerOverGameObject(InputExtensions.IsUsingTouch ? Input.GetTouch(0).fingerId : -1))
			TapToPlay();
	}

	private void EnableVictoryObjects()
	{
		if(defeat.activeSelf) return;
		
		victory.SetActive(true);
		nextLevelButton.interactable = false;
		constantRetryButton.SetActive(false);
		
		AudioManager.instance.Play("Win");
	}

	private void EnableLossObjects()
	{
		if(victory.activeSelf) return;

		if (_hasLost) return;
		
		red.enabled = true;
		var originalColor = red.color;
		red.color = Color.clear;
		red.DOColor(originalColor, 1f);

		defeat.SetActive(true);
		retry.SetActive(true);
		skipLevel.SetActive(true);
		constantRetryButton.SetActive(false);
		_hasLost = true;
		
		AudioManager.instance.Play("Lose");
	}

	public void EnableNextLevel()
	{
		nextLevelButton.interactable = true;
	}
	
	private void TapToPlay()
	{
		_hasTapped = true;
		holdToAim.SetActive(false);
		skipLevel.SetActive(false);
		
		if(GameEvents.Only)
			GameEvents.Only.InvokeTapToPlay();
	}

	public void Retry()
	{
		if(ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		AudioManager.instance.Play("Button");
	}
	
	public void NextLevel()
	{
		if(ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		
		if (PlayerPrefs.GetInt("levelNo", 1) < lastRegularLevel + 1)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, lastRegularLevel + 1);
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		
		AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);
	}

	public void SkipLevel()
	{
		if(!ApplovinManager.instance.TryShowRewardedAds()) return;

		AdsMediator.StartListeningForAds(this);
	}

	public void ABToggle(bool status)
	{
		InputHandler.Only.ShouldUseTapAndPunch(status);
	}

	private void OnEnemyReachPlayer()
	{
		if(GAScript.Instance)
			GAScript.Instance.LevelFail(PlayerPrefs.GetInt("levelNo").ToString());
		
		Invoke(nameof(EnableLossObjects), 1.5f);
	}

	private void OnGameEnd()
	{
		if(GAScript.Instance)
			GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("levelNo").ToString());
		
		Invoke(nameof(EnableVictoryObjects), 1f);
	}

	private void AdRewardRecieveBehaviour()
	{
		if (PlayerPrefs.GetInt("levelNo", 1) < lastRegularLevel + 1)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}
		else
		{
			var x = Random.Range(5, lastRegularLevel + 1);
			PlayerPrefs.SetInt("lastBuildIndex", x);
			SceneManager.LoadScene(x);
		}

		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);

		ShopStateController.ShopStateSerializer.SaveCurrentState();

		AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);

		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		AdRewardRecieveBehaviour();
	}

	public void OnShowDummyAd()
	{
		AdRewardRecieveBehaviour();
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		AdsMediator.StopListeningForAds(this);
	}
}
