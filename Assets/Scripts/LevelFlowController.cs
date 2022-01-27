using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController only;

	public bool isGiantLevel;
	[SerializeField] private List<int> enemiesInArea;
	[SerializeField] private List<Transform> throwTargetsInArea;
	
	public int enemiesInCurrentArea, enemiesKilledInCurrentArea;
	public int currentArea;

	private int _totalEnemiesRemaining;

	private void OnEnable()
	{
		GameEvents.only.enemyKilled += OnEnemyKilled;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyKilled -= OnEnemyKilled;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		if (enemiesInArea.Count == 0)
		{
			Debug.LogWarning("Level Flow Controller values not changed");
			Debug.Break();
		}

		enemiesInCurrentArea = enemiesInArea[currentArea];
		enemiesKilledInCurrentArea = 0;

		foreach (var area in enemiesInArea)
			_totalEnemiesRemaining += area;

		DOTween.KillAll();
		Vibration.Init();
	}

	private IEnumerator WaitBeforeMovingToNextArea()
	{
		//were waiting so that the wrist ctrler can return to arm

		const float timer = 1f;
		var elapsed = 0f;
		const float intervals = 0.2f;
		while (!InputHandler.Only.IsInIdleState())
		{
			elapsed += intervals;
			yield return GameExtensions.GetWaiter(intervals);
			if(timer < elapsed) break;
		}

		if (currentArea == enemiesInArea.Count - 1)
			GameEvents.only.InvokeGameEnd();
		else
		{
			GameEvents.only.InvokeMoveToNextArea();
			InputHandler.Only.AssignDisabledState();
		}
	}

	private void OnEnemyKilled()
	{
		enemiesKilledInCurrentArea++;
		_totalEnemiesRemaining--;
		if (enemiesKilledInCurrentArea >= enemiesInCurrentArea)
			MoveToNextArea();
	}

	private void MoveToNextArea()
	{
		StartCoroutine(WaitBeforeMovingToNextArea());
	}

	private void OnReachNextArea()
	{
		enemiesInCurrentArea = enemiesInArea[++currentArea];
		enemiesKilledInCurrentArea = 0;
	}

	public bool IsThisLastEnemy()
	{
		return _totalEnemiesRemaining == 1;
	}

	public bool IsThisLastEnemyOfArea()
	{
		return enemiesInArea[currentArea] - enemiesKilledInCurrentArea == 1;
	}

	public bool DidKillLastEnemyOfArea()
	{
		return enemiesInArea[currentArea] == enemiesKilledInCurrentArea;
	}

	public bool IsInGiantFight()
	{
		return IsThisLastEnemy() && isGiantLevel;
	}

	public bool TryGetCurrentThrowTarget(out Transform target)
	{
		target = throwTargetsInArea[currentArea] ? throwTargetsInArea[currentArea] : null;
		
		return target;
	}
}
