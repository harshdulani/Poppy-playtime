using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChaseLevelMainCanvasController : MonoBehaviour
{
    public int lastRegularLevel;
	
	[SerializeField] private GameObject holdToAim, victory, defeat, retry, constantRetryButton, skipLevel;
	[SerializeField] private TextMeshProUGUI levelText, instructionText;
	[SerializeField] private Image red;
	[SerializeField] private Toggle abToggle;
	[SerializeField] private string tapInstruction, swipeInstruction;

	[SerializeField] private Button nextLevelButton;

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

		var levelNo = PlayerPrefs.GetInt("levelNo", 1);
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

	private void EnableVictoryObjects()
	{
		if(defeat.activeSelf) return;
		
		victory.SetActive(true);
		//nextLevelButton.interactable = false;
		nextLevelButton.gameObject.SetActive(true);
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
		//skipLevel.SetActive(true);
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

	private void RetryBehaviour()
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
			YcHelper.ShowInterstitial(NextLevelBehaviour);
			return;
		}
		
		NextLevelBehaviour();
	}

	private void NextLevelBehaviour()
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

	public void ShowGDPRButton()
	{
		YcHelper.ShowGDPR();
	}
}
