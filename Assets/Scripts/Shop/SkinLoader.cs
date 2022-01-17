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
	[SerializeField] private Sprite[] coloredWeaponSprites, blackWeaponSprites;
	[SerializeField] private Button okayButton, skipButton, claimButton;
	[SerializeField] private Image coloredWeaponImage, blackWeaponImage;
	[SerializeField] private TextMeshProUGUI percentageUnlockedText;

	[SerializeField] private GameObject loaderPanel, lockedButtonsHolder, unlockedButtonsHolder; 
	
	[SerializeField] private int levelsPerUnlock = 5;
	[SerializeField] private float tweenDuration;

	private static int _currentSkinInUse = 0;
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

	private void Start()
	{
		Initialise();
		
		loaderPanel.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			OnGameEnd();
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
	
	public static WeaponType GetCurrentSkin() => (WeaponType)PlayerPrefs.GetInt("currentSkinInUse", 0);

	public void Okay()
	{
		loaderPanel.SetActive(false);
		//play exit animatn
	}
	
	public void Skip()
	{
		loaderPanel.SetActive(false);
		_currentSkinBeingUnlocked++;
		_currentSkinPercentageUnlocked = 0f;
		//play animatn
		
		PlayerPrefs.SetInt("currentSkinBeingUnlocked", _currentSkinBeingUnlocked);
		PlayerPrefs.SetFloat("currentSkinPercentageUnlocked", _currentSkinPercentageUnlocked);
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
	}
	
	private void OnGameEnd()
	{
		if(_currentSkinBeingUnlocked >= Enum.GetNames(typeof(WeaponType)).Length) return;
		
		Invoke(nameof(ShowPanel), 1f);
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
			okayButton.interactable = true;
			skipButton.interactable = true;
			claimButton.interactable = true;
		});

		if (_currentSkinPercentageUnlocked < 0.99f)
		{
			lockedButtonsHolder.SetActive(true);
			unlockedButtonsHolder.SetActive(false);
			return;
		}
		lockedButtonsHolder.SetActive(false);
		unlockedButtonsHolder.SetActive(true);
		//confetti particle fx for complete loader
	}
}