using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum WeaponType
{
	Punch,
	Hammer,
	Boot,
	Heel,
	Gun,
	Shield,
	Sneaker
}

public class SkinLoader : MonoBehaviour
{
	public static SkinLoader only;
	
	[SerializeField] private Sprite[] coloredWeaponSprites, blackWeaponSprites;
	[SerializeField] private Button  skipButton, claimButton;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText;

	[SerializeField] private GameObject loaderPanel, unlockedButtonsHolder; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration, panelOpenWait;

	private int _currentSkinInUse;
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

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
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
		_currentSkinInUse = PlayerPrefs.GetInt("currentSkinInUse", 0);

		if(_currentSkinBeingUnlocked >= Enum.GetNames(typeof(WeaponType)).Length) return;
		
		coloredWeaponImage.sprite = coloredWeaponSprites[_currentSkinBeingUnlocked];
		blackWeaponImage.sprite = blackWeaponSprites[_currentSkinBeingUnlocked];
		percentageUnlockedText.text = (int)(_currentSkinPercentageUnlocked * 100) + "%";

		blackWeaponImage.fillAmount = 1 - _currentSkinPercentageUnlocked;
	}

	public Sprite GetSkinSprite(int index = -1)
	{
		if (index == -1)
			return coloredWeaponSprites[PlayerPrefs.GetInt("currentSkinInUse", 0)];
		
		if (index >= coloredWeaponSprites.Length)
			return coloredWeaponSprites[^1];

		return coloredWeaponSprites[index];
	}

	public WeaponType GetSkinName(int index = -1) => (WeaponType) (index == -1 ? PlayerPrefs.GetInt("currentSkinInUse", 0) : index);

	public void UpdateSkinInUse(int currentSkin)
	{
		_currentSkinInUse = currentSkin;
		_currentSkinBeingUnlocked = _currentSkinInUse + 1;
		
		PlayerPrefs.SetInt("currentSkinInUse", _currentSkinInUse);
		PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
		ResetLoader();
	}

	public void Skip()
	{
		loaderPanel.SetActive(false);
		_currentSkinBeingUnlocked++;
		//play animatn
		ResetLoader();
		
		GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>().NextLevel();
	}

	public void Claim()
	{
		_currentSkinInUse++;
		_currentSkinBeingUnlocked++;
		_currentSkinPercentageUnlocked = 0f;
		//confetti
		
		loaderPanel.SetActive(false);
		
		PlayerPrefs.SetInt("currentSkinInUse", _currentSkinInUse);
		PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
		
		GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<MainCanvasController>().NextLevel();
	}

	private void ResetLoader()
	{
		_currentSkinPercentageUnlocked = 0f;
		
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

	public bool ShouldShowNextLevel() => _currentSkinPercentageUnlocked < 0.99f;
	
	private void OnGameEnd()
	{
		if(_currentSkinBeingUnlocked >= Enum.GetNames(typeof(WeaponType)).Length) return;
		
		Invoke(nameof(ShowPanel), panelOpenWait);
	}
}