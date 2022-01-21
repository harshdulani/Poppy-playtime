using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject holdToAim, victory, defeat, nextLevel, retry, constantRetryButton;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private Image red;
	[SerializeField] private Toggle abToggle;
	
	private bool _hasTapped, _hasLost;
	
	private void OnEnable()
	{
		GameEvents.only.enemyKillPlayer += OnEnemyReachPlayer;
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyKillPlayer -= OnEnemyReachPlayer;
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void Start()
	{
		levelText.text = "Level " + PlayerPrefs.GetInt("levelNo", 1);
		abToggle.isOn = PlayerPrefs.GetInt("controlMechanic", 0) == 1;
		
		if(GAScript.Instance)
			GAScript.Instance.LevelStart(PlayerPrefs.GetInt("levelNo", 0).ToString());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(_hasTapped) return;

		if(EventSystem.current.IsPointerOverGameObject()) return;
		
		if (InputExtensions.GetFingerHeld() || InputExtensions.GetFingerDown()) TapToPlay();
	}

	private void TapToPlay()
	{
		_hasTapped = true;
		holdToAim.SetActive(false);
		
		if(GameEvents.only)
			GameEvents.only.InvokeTapToPlay();
	}

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		AudioManager.instance.Play("Button");
	}
	
	public void NextLevel()
	{
		//AudioManager.instance.Play("button");
		if (PlayerPrefs.GetInt("levelNo", 1) < SceneManager.sceneCountInBuildSettings - 1)
		{
			var x = PlayerPrefs.GetInt("levelNo", 1) + 1;
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		else
		{
			var x = Random.Range(5, SceneManager.sceneCountInBuildSettings - 1);
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo", 1) + 1);
		
		AudioManager.instance.Play("Button");
		Vibration.Vibrate(15);
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

	private void EnableVictoryObjects()
	{
		victory.SetActive(true);
		nextLevel.SetActive(SkinLoader.only.ShouldShowNextLevel());
		constantRetryButton.SetActive(false);
		
		AudioManager.instance.Play("Win");
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
		constantRetryButton.SetActive(false);
		_hasLost = true;
		
		AudioManager.instance.Play("Lose");
	}
}
