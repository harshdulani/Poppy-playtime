using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour//, IWantsAds
{
	public int lastRegularLevel;
	
	[SerializeField] private GameObject holdToAim, defeat, retry, constantRetryButton, skipLevel;
	[SerializeField] private TextMeshProUGUI levelText, instructionText;
	[SerializeField] private Image red;
	[SerializeField] private Toggle abToggle;
	[SerializeField] private string tapInstruction, swipeInstruction;

	private LevelIndicator _indicator;
	private bool _hasLost;

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
		GameEvents.Only.GameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
		GameEvents.Only.GameEnd -= OnGameEnd;
	}

	private void Start()
	{
		_indicator = GetComponent<LevelIndicator>();

		var levelNo = PlayerPrefs.GetInt("levelNo", 1) - PlayerPrefs.GetInt("encounteredBonusLevels", 0);
		levelText.text = "Level " + levelNo;
		abToggle.isOn = PlayerPrefs.GetInt("isUsingTapAndPunch", 0) == 0;
		instructionText.text = abToggle.isOn ? tapInstruction : swipeInstruction;
		
		if(levelNo < 5)
		{
			skipLevel.SetActive(false);
			_indicator.SetIndicatorEnable(false);
		}

		PlayerPrefs.SetInt("lastRegularLevel", lastRegularLevel);
		
		//if(GAScript.Instance)
		//GAScript.Instance.LevelStart(PlayerPrefs.GetInt("levelNo", 0).ToString());

		if(YcHelper.InstanceExists)
			YcHelper.LevelStart(PlayerPrefs.GetInt("levelNo", 0));
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.N)) NextLevel();
		
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void EnableLossObjects()
	{
		if (_hasLost) return;
		
		red.enabled = true;
		var originalColor = red.color;
		red.color = Color.clear;
		red.DOColor(originalColor, 1f);

		defeat.SetActive(true);
		retry.SetActive(true);
		//skipLevel.SetActive(true);Pl
		constantRetryButton.SetActive(false);
		_hasLost = true;
		
		AudioManager.instance.Play("Lose");
	}

	public void Retry()
	{
		/*
		if(ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		*/
		if (YcHelper.InstanceExists && YcHelper.IsAdAvailable())
		{
			YcHelper.ShowInterstitial(RetryBehaviour);
			return;
		}

		RetryBehaviour();
	}

	private static void RetryBehaviour()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		AudioManager.instance.Play("Button");
	}

	public void NextLevel()
	{
		/*
		if(ApplovinManager.instance)
			if(ApplovinManager.instance.enableAds)
				ApplovinManager.instance.ShowInterstitialAds();
		*/

		if (YcHelper.InstanceExists && YcHelper.IsAdAvailable())
		{
			YcHelper.ShowInterstitial(() => SceneChangeManager.LoadNextScene());
			return;
		}
		
		SceneChangeManager.LoadNextScene();
	}

	public void SkipLevel()
	{
		/*if(!ApplovinManager.instance.TryShowRewardedAds()) return;
		AdsMediator.StartListeningForAds(this);*/
		
		if(YcHelper.InstanceExists && YcHelper.IsAdAvailable())
			YcHelper.ShowRewardedAds(AdRewardRecieveBehaviour);
	}

	public void ABToggle(bool status)
	{
		InputHandler.Only.ShouldUseTapAndPunch(status);
	}
	
	public void ShowGDPRButton()
	{
		if(YcHelper.InstanceExists)
			YcHelper.ShowGDPR();
	}

	private void OnTapToPlay()
	{
		holdToAim.SetActive(false);
		skipLevel.SetActive(false);
	}

	private void OnEnemyReachPlayer()
	{
		//if(GAScript.Instance)
		//	GAScript.Instance.LevelFail(PlayerPrefs.GetInt("levelNo").ToString());

		if (YcHelper.InstanceExists) 
			YcHelper.LevelEnd(false);

		DOVirtual.DelayedCall(1.5f, EnableLossObjects);
	}

	private void OnGameEnd()
	{
		//if(GAScript.Instance)
		//GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("levelNo").ToString());

		if (YcHelper.InstanceExists) 
			YcHelper.LevelEnd(true);
		
		_hasLost = false;
	}

	private void AdRewardRecieveBehaviour()
	{
		SceneChangeManager.LoadNextScene();
		//AdsMediator.StopListeningForAds(this);
	}

	/*
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
	}*/
}