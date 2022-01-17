using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController only;

	public bool isGiantLevel;
	[SerializeField] private List<int> enemiesInArea;

	public int enemiesInCurrentArea, enemiesKilledInCurrentArea;
	public int currentArea;

	private GiantController _giant;
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

		if (isGiantLevel)
			_giant = GameObject.FindGameObjectWithTag("Giant").GetComponent<GiantController>();
		
		DOTween.KillAll();
		Vibration.Init();
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
		if (currentArea == enemiesInArea.Count - 1)
			GameEvents.only.InvokeGameEnd();
		else
		{
			GameEvents.only.InvokeMoveToNextArea();
			InputHandler.Only.AssignDisabledState();
		}
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

	public bool DidKillLastEnemyOfArea()
	{
		return enemiesInArea[currentArea] - enemiesKilledInCurrentArea == 0;
	}

	public bool IsInGiantFight()
	{
		return IsThisLastEnemy() && isGiantLevel;
	}

	public GiantController GetGiant() => _giant;
}
