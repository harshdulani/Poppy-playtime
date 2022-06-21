using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class ComboCanvasManager : MonoBehaviour
{
	[Header("Animation"), SerializeField] private float totalTextAnimationDuration;
	[SerializeField] private float holdTextVisibilityDuration = 0.8f;

	[Header("AB Testing"), SerializeField] private bool isTestingEnlargedComboText;
	[SerializeField] private float testingTextSizeMultiplier = 1;

	[SerializeField] private TMP_Text comboText, exclamationText, propDestroyedText;
	[SerializeField] private List<string> exclamations, singleHitComboWords;
	
	[SerializeField] private float countDownTime;
	
	private float _timeLeft, _chainReactionTimeLeft;
	private int _count, _enemyCount;
	private bool _startTimer, _startChainReactionTimer;

	private void OnEnable()
	{
		GameEvents.Only.PunchHit += OnPunchHit;
		GameEvents.Only.EnemyKilled += OnEnemyKilled;
		GameEvents.Only.PropHitsEnemy += OnPropHitsEnemy;
	}

	private void OnDisable()
	{
		GameEvents.Only.PunchHit -= OnPunchHit;
		GameEvents.Only.EnemyKilled -= OnEnemyKilled;
		GameEvents.Only.PropHitsEnemy -= OnPropHitsEnemy;
	}

	private void Start()
    {
		_timeLeft = countDownTime;

		if (!Application.isEditor)
			isTestingEnlargedComboText = false;
		
		//ab testing
		if ((!YcHelper.InstanceExists || !YcHelper.GetIsComboTextTestingOn()) && (!Application.isEditor || !isTestingEnlargedComboText)) return;
		
		comboText.fontSize *= testingTextSizeMultiplier;
		exclamationText.fontSize *= testingTextSizeMultiplier;
		propDestroyedText.fontSize *= testingTextSizeMultiplier;
	}

	private void Update()
	{
		if (_startTimer) ComboTimer();
		if (_startChainReactionTimer) ChainReactionTimer();
	}

	private void ComboPopUp()
	{
		if (_count == 0)
		{
			_timeLeft = countDownTime;
			_startTimer = true;
			
			comboText.text = exclamations[Random.Range(0, exclamations.Count - 1)];

			PlayComboAnimation(comboText);
			_count += 1;
		}
		else
		{
			_timeLeft = countDownTime;
			_count += 1;
			comboText.text = "Combo x" + _count;
			PlayComboAnimation(comboText);

			exclamationText.text = exclamations[Random.Range(0, singleHitComboWords.Count - 1)];
			PlayComboAnimation(exclamationText);
		}
	}

	private void ChainReaction()
	{
		if (!_startTimer) return;
		
		_count++;
		if (_enemyCount <= 1) return;
		
		_startChainReactionTimer = true;
		
		exclamationText.text = exclamations[Random.Range(0, exclamations.Count - 1)];
		PlayComboAnimation(exclamationText);

		DOVirtual.DelayedCall(0.5f, () =>
		{
			comboText.text = "Chain Reaction x" + _enemyCount;
			PlayComboAnimation(comboText);
		});
	}

	private void ComboTimer()
	{
		_timeLeft -= Time.deltaTime;

		if (!(_timeLeft < 0f)) return;
		
		_count = 0;
		_enemyCount = 0;
		_startTimer = false;
	}
	
	private void ChainReactionTimer()
	{
		_chainReactionTimeLeft -= Time.deltaTime;

		if (_chainReactionTimeLeft < 0)
			_startChainReactionTimer = false;
	}
	
	private void PlayComboAnimation(TMP_Text text)
	{
		//make sure no other tween is playing
		var tweenList = DOTween.TweensByTarget(text);
		if (tweenList != null && tweenList.Count > 0)
			foreach (var tween in tweenList)
				tween.Kill();

		var seq = DOTween.Sequence();

		var initColor = text.color;
		var destColor = initColor;
		destColor.a = 1f;
		
		seq.Append(text.DOColor(destColor, totalTextAnimationDuration * 0.75f));

		var initScale = text.transform.localScale;
		
		seq.Join(text.transform.DOScale(initScale * 1.15f, totalTextAnimationDuration * 0.75f));

		seq.AppendInterval(holdTextVisibilityDuration);

		seq.Append(text.DOColor(initColor, totalTextAnimationDuration * 0.25f));
		seq.Join(text.transform.DOScale(initScale, totalTextAnimationDuration * 0.25f));
	}
	
	private static void PlayBarrelHitAnimation(TMP_Text text)
	{
		var seq = DOTween.Sequence();

		var initColor = text.color;
		var destColor = initColor;
		destColor.a = 1f;
		
		seq.Append(text.DOColor(destColor, 0.15f));

		var initScale = text.transform.localScale;
		seq.Join(text.transform.DOScale(initScale * 1.15f, 0.15f));

		seq.AppendInterval(1f);

		seq.Append(text.DOColor(initColor, 0.3f));
		seq.Join(text.transform.DOScale(initScale, 0.3f));
	}

	private void OnPunchHit() => ComboPopUp();

	private void OnPropHitsEnemy() => PlayBarrelHitAnimation(propDestroyedText);

	private void OnEnemyKilled()
	{
		ChainReaction();
		_enemyCount += 1;
	}
}