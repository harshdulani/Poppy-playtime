using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ThemeInfo
{
	public Color backgroundCircleColor, foregroundCircleColor, activeCircleColor, progressBarColor;
}

public class LevelIndicator : MonoBehaviour
{
	[SerializeField] private int levelsPerTheme = 7;
	[SerializeField] private GameObject indicator;
	[SerializeField] private TextMeshProUGUI plainText;
	[SerializeField] private RectTransform progressBarFillRect;
	[SerializeField] private Image progressBarFill;
	[SerializeField] private Transform circlesRoot;
	[SerializeField] private ThemeInfo theme;
	[SerializeField] private float inactiveLevelScale = 0.75f;

	private List<Image> _backgroundCircles, _foregroundCircles;
	private List<TextMeshProUGUI> _levelNums;
	private List<Transform> _textRects;

	private MainCanvasController _mainCanvas;
	private ChaseLevelMainCanvasController _chaseMainCanvas;
	private bool _isChaseLevel;

	private void OnEnable()
	{
		GameEvents.Only.ReachNextArea += OnReachNextArea;
	}
	
	private void OnDisable()
	{
		GameEvents.Only.ReachNextArea -= OnReachNextArea;
	}

	private void Start()
	{
		_backgroundCircles = new List<Image>();
		_foregroundCircles = new List<Image>();
		_levelNums = new List<TextMeshProUGUI>();
		_textRects = new List<Transform>();
		if (!TryGetComponent(out _mainCanvas))
		{
			TryGetComponent(out _chaseMainCanvas);
			_isChaseLevel = true;
		}

		for (var i = 0; i < levelsPerTheme; i++)
		{
			var currentChild = circlesRoot.GetChild(i);
			_backgroundCircles.Add(currentChild.GetComponent<Image>());
			_foregroundCircles.Add(currentChild.GetChild(0).GetComponent<Image>());

			var levelText = currentChild.GetChild(1);
			
			//if level text is disabled, that means this child has an image for spcl level/boss level
			var textActiveStatus = levelText.gameObject.activeSelf;

			_levelNums.Add(textActiveStatus ? levelText.GetComponent<TextMeshProUGUI>() : null);
			_textRects.Add(textActiveStatus ? levelText : currentChild.GetChild(2));
		}
		
		if(MakeCurrentLevel()) return;
		
		indicator.SetActive(false);
		var levelNo = PlayerPrefs.GetInt("levelNo", 1) - PlayerPrefs.GetInt("encounteredBonusLevels", 0);
		plainText.text = "Level " + levelNo;
		plainText.gameObject.SetActive(true);
	}

	private bool MakeCurrentLevel()
	{
		var currentLevel = PlayerPrefs.GetInt("levelNo") - PlayerPrefs.GetInt("encounteredBonusLevels", 0);
		var lastRegLevel = _isChaseLevel ? _chaseMainCanvas.lastRegularLevel : _mainCanvas.lastRegularLevel;
		
		print($"total {PlayerPrefs.GetInt("levelNo")} bonus {PlayerPrefs.GetInt("encounteredBonusLevels", 0)}");
		// if you don't have theme info for any more levels, do not show indicator - show plain old text instead.
		if (currentLevel > lastRegLevel) return false;

		var currentThemedLevel = (currentLevel - 1) % levelsPerTheme;
		var currentTheme = Mathf.FloorToInt((currentLevel - 1) / (float) levelsPerTheme);
		
		for (var i = 0; i < levelsPerTheme; i++)
		{
			_backgroundCircles[i].color = theme.backgroundCircleColor;
			_foregroundCircles[i].color = i < currentThemedLevel ? theme.foregroundCircleColor : Color.clear;
			
			if(_levelNums[i])
				_levelNums[i].text = ((currentTheme * levelsPerTheme) + i + 1).ToString();

			if (i != currentThemedLevel)
			{
				//purposely multiplying with current scaling to make up for bigger sprites etc like star 
				_textRects[i].localScale *= inactiveLevelScale;
				continue;
			}

			_foregroundCircles[i].color = theme.activeCircleColor;
		}
		
		progressBarFill.color = theme.progressBarColor;
		progressBarFillRect.anchorMax = new Vector2(currentThemedLevel / (float)levelsPerTheme, progressBarFillRect.anchorMax.y);
		return true;
	}

	public void SetIndicatorEnable(bool status) => plainText.transform.parent.gameObject.SetActive(status);

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsInGiantFight()) return;
		
		indicator.SetActive(false);
		plainText.gameObject.SetActive(false);
	}
}