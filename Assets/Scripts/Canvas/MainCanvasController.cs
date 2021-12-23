using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject holdToAim, victory, nextLevel;
	[SerializeField] private TextMeshProUGUI levelText;

	private bool _hasTapped;
	
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
		levelText.text = "Level " + PlayerPrefs.GetInt("levelNo");
		
		// if(GAScript.Instance)
		// 	GAScript.Instance.LevelStart(PlayerPrefs.GetInt("levelNo").ToString());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
		if(_hasTapped) return;

		if (InputExtensions.GetFingerHeld() || InputExtensions.GetFingerDown()) TapToPlay();
	}

	public void TapToPlay()
	{
		//AudioManager.instance.Play("button");
		_hasTapped = true;
		holdToAim.SetActive(false);
		
		if(GameEvents.only)
			GameEvents.only.InvokeTapToPlay();
	}

	public void NextLevel()
	{
		//AudioManager.instance.Play("button");
		if (PlayerPrefs.GetInt("levelNo") < SceneManager.sceneCountInBuildSettings - 1)
		{
			var x = PlayerPrefs.GetInt("levelNo") + 1;
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		else
		{
			var x = Random.Range(5, SceneManager.sceneCountInBuildSettings - 1);
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo") + 1);
	}

	private void OnGameEnd()
	{
		Invoke(nameof(EnableVictoryObjects), 1f);
	}

	private void EnableVictoryObjects()
	{
		victory.SetActive(true);
		nextLevel.SetActive(true);
	}
}
