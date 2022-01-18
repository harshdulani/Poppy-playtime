using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComboCanvasManager : MonoBehaviour
{
	[SerializeField] private int count = 0,enemyCount = 0;
	[SerializeField] private TMP_Text comboText;
	[SerializeField] private TMP_Text exclamationText;
	[SerializeField] private TMP_Text propDestroyedText;
	[SerializeField] private List<string> exclamations;
	
	[SerializeField] private Animator comboAnimator,exclamationAnimator;
	[SerializeField] private Animation barrelHitAnimation;
	
	[SerializeField] private bool startTimer;
	[SerializeField] private float countDownTime;
	[SerializeField] private bool startChainReactionTimer;
	
	private static readonly int ComboAnimationStart = Animator.StringToHash("toFadeIn");
	private static readonly int ComboAnimationEnd = Animator.StringToHash("toFadeOut");
	
	private float _timeLeft;
	private float _chainReactionTimeLeft;
	
	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.enemyKilled += OnEnemyKilled;
		GameEvents.only.propHitsEnemy += OnPropHitsEnemy;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.punchHit -= OnEnemyKilled;
		GameEvents.only.propHitsEnemy -= OnPropHitsEnemy;

	}

	private void Start()
    {
        comboText.gameObject.SetActive(false);
		exclamationText.gameObject.SetActive(false);
		propDestroyedText.gameObject.SetActive(false);
		_timeLeft = countDownTime;
	}

	private void Update()
	{
		if (startTimer)
		{
			ComboTimer();
		}

		if (startChainReactionTimer)
		{
			ChainReactionTimer();
		}
	}
	
	private void OnPunchHit()
	{
		StartCoroutine(ComboPopUp());
	}
	
	private void OnEnemyKilled()
	{
		StartCoroutine(ChainReaction());
		enemyCount += 1;
	}

	private void OnPropHitsEnemy()
	{
		StartCoroutine(PropHitRoutine());
	}

	private IEnumerator ComboPopUp()
	{
		if (count == 0)
		{
			_timeLeft = countDownTime;
			startTimer = true;
			
			comboText.text = "Nice!";
			comboText.gameObject.SetActive(true);
			StartCoroutine(PlayComboAnimation(comboAnimator));
			
			count += 1;
		}
		else
		{
			_timeLeft = countDownTime;
			
			comboText.gameObject.SetActive(true);
			StartCoroutine(PlayComboAnimation(comboAnimator));
			
			count += 1;
			comboText.text = "Combo " + "x" + count;
			
			exclamationText.gameObject.SetActive(true);
			comboAnimator.SetTrigger(ComboAnimationStart);
			StartCoroutine(PlayComboAnimation(exclamationAnimator));
			exclamationText.text = exclamations[Random.Range(0, exclamations.Count - 1)];
		}
		
		yield return new WaitForSeconds(2f);
		
		comboText.gameObject.SetActive(false);
		exclamationText.gameObject.SetActive(false);
	}

	private IEnumerator ChainReaction()
	{
		if (!startTimer) yield break;
		
		count++;
		if (enemyCount <= 1) yield break;
		
		startChainReactionTimer = true;
				
		exclamationText.gameObject.SetActive(true);
		StartCoroutine(PlayComboAnimation(exclamationAnimator));
		exclamationText.text = exclamations[Random.Range(0, exclamations.Count - 1)];
				
		yield return new WaitForSeconds(0.5f);
				
		exclamationText.gameObject.SetActive(false);
		comboText.gameObject.SetActive(true);
		comboText.text = "Chain Reaction x" + enemyCount;
		StartCoroutine(PlayComboAnimation(comboAnimator));
	}

	private IEnumerator PropHitRoutine()
	{
		propDestroyedText.gameObject.SetActive(true);
		barrelHitAnimation.Play();
		
		yield return new WaitForSeconds(1f);
		propDestroyedText.gameObject.SetActive(false);
	}

	private void ComboTimer()
	{
		_timeLeft -= Time.deltaTime;
		
		if (_timeLeft < 0f)
		{
			count = 0;
			enemyCount = 0;
			startTimer = false;
		}
	}
	
	private void ChainReactionTimer()
	{
		_chainReactionTimeLeft -= Time.deltaTime;

		if (_chainReactionTimeLeft < 0)
			startChainReactionTimer = false;
	}

	private IEnumerator PlayComboAnimation(Animator animator)
	{
		animator.SetTrigger(ComboAnimationStart);
		yield return new WaitForSeconds(1f);
		animator.SetTrigger(ComboAnimationEnd);
	}
}
