using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct ThemeInfo
{
	public Color backgroundCircleColor, foregroundCircleColor, activeCircleColor, progressBarColor;
}

public class LevelIndicator : MonoBehaviour
{
	[SerializeField] private GameObject indicator, plainText;
	[SerializeField] private RectTransform progressBarFillRect;
	[SerializeField] private Image progressBarFill;
	[SerializeField] private Transform circlesRoot;
	[SerializeField] private ThemeInfo theme;
	[SerializeField] private float inactiveLevelScale = 0.75f;

	private List<Image> _backgroundCircles, _foregroundCircles;
	private List<Transform> _textRects;
	private const int LevelsPerTheme = 7;
	
	private static int GetBuildIndex() => SceneManager.GetActiveScene().buildIndex;

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
		_textRects = new List<Transform>();

		for (var i = 0; i < LevelsPerTheme; i++)
		{
			_backgroundCircles.Add(circlesRoot.GetChild(i).GetComponent<Image>());
			_foregroundCircles.Add(circlesRoot.GetChild(i).GetChild(0).GetComponent<Image>());

			var levelText = circlesRoot.GetChild(i).GetChild(1);
			_textRects.Add(levelText.gameObject.activeSelf ? levelText : circlesRoot.GetChild(i).GetChild(2));
		}
		
		if(MakeCurrentLevel()) return;

		indicator.SetActive(false);
		plainText.SetActive(true);
	}

	private bool MakeCurrentLevel()
	{
		var currentLevel = GetBuildIndex() - 1;

		// if you don't have theme info for any more levels, do not show indicator - show plain old text instead.
		if (SceneManager.sceneCountInBuildSettings < PlayerPrefs.GetInt("levelNo")) return false;

		var currentThemedLevel = currentLevel % LevelsPerTheme;
		
		for (var i = 0; i < LevelsPerTheme; i++)
		{
			_backgroundCircles[i].color = theme.backgroundCircleColor;
			_foregroundCircles[i].color = i < currentThemedLevel ? theme.foregroundCircleColor : Color.clear;

			if (i != currentThemedLevel)
			{
				//purposely multiplying with current scaling to make up for bigger sprites etc like star 
				_textRects[i].localScale *= inactiveLevelScale;
				continue;
			}

			_foregroundCircles[i].color = theme.activeCircleColor;
		}
		
		progressBarFill.color = theme.progressBarColor;
		progressBarFillRect.anchorMax = new Vector2(currentThemedLevel / (float)LevelsPerTheme, progressBarFillRect.anchorMax.y);
		return true;
	}

	public void SetIndicatorEnable(bool status) => plainText.transform.parent.gameObject.SetActive(status);

	private void OnReachNextArea()
	{
		if(!LevelFlowController.only.IsInGiantFight()) return;
		
		indicator.SetActive(false);
		plainText.SetActive(false);
	}
}