using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class ComboCanvasManager : MonoBehaviour
{
	[SerializeField] private int count = 0,enemyCount = 0;
	[SerializeField] private TMP_Text comboText;
	[SerializeField] private TMP_Text exclamationText;
	[SerializeField] private TMP_Text propDestoryedText;
	[SerializeField] private List<string> exclamations;

	[SerializeField] private Animation comboAnimation;
	[SerializeField] private Animation exclamationAnimation;
	[SerializeField] private Animation barrelHitAnimation;
	[SerializeField] private bool startTimer;
	[SerializeField] private float countDownTime;

	[SerializeField] private bool startChainReactionTimer;

	private float _timeLeft;
	private float _chainReactionTimeLeft;
	
	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.enemyKilled += OnEnemyKilled;
		GameEvents.only.propDestroyed += OnPropDestroyed;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.punchHit -= OnEnemyKilled;
		GameEvents.only.propDestroyed -= OnPropDestroyed;

	}

	private void Start()
    {
        comboText.gameObject.SetActive(false);
		exclamationText.gameObject.SetActive(false);
		propDestoryedText.gameObject.SetActive(false);
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

	private void OnPropDestroyed(Transform propDestroyed)
	{
		if (propDestroyed.TryGetComponent(out PropController prop))
		{
			StartCoroutine(PropHitRoutine());
		}
	}
	
	IEnumerator ComboPopUp()
	{
		if (count == 0)
		{
			_timeLeft = countDownTime;
			startTimer = true;
			comboText.text = "Nice!";
			comboText.gameObject.SetActive(true);
			comboAnimation.Play();
			count += 1;
		}
		else
		{
			_timeLeft = countDownTime;
			comboText.gameObject.SetActive(true);
			comboAnimation.Play();
			count += 1;
			comboText.text = "Combo " + "x" + count;
			exclamationText.gameObject.SetActive(true);
			exclamationAnimation.Play();
			exclamationText.text = exclamations[Random.Range(0, exclamations.Count - 1)];
		}
		
		yield return new WaitForSeconds(1f);
		
		comboText.gameObject.SetActive(false);
		exclamationText.gameObject.SetActive(false);
	}

	private IEnumerator ChainReaction()
	{
		if (startTimer)
		{
			count++;
			
			if (enemyCount > 1)
			{
				startChainReactionTimer = true;
				
				exclamationText.gameObject.SetActive(true);
				exclamationAnimation.Play();
				exclamationText.text = exclamations[Random.Range(0, exclamations.Count - 1)];
				
				yield return new WaitForSeconds(0.5f);
				
				exclamationText.gameObject.SetActive(false);
				comboText.gameObject.SetActive(true);
				comboText.text = "Chain Reaction x" + enemyCount;
				comboAnimation.Play();
			}
		}
	}

	private IEnumerator PropHitRoutine()
	{
		propDestoryedText.gameObject.SetActive(true);
		barrelHitAnimation.Play();
		yield return new WaitForSeconds(1f);
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
}
