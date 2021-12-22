using System.Collections.Generic;
using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	[SerializeField] private List<int> enemiesInArea;

	public int enemiesInCurrentArea, enemiesKilledInCurrentArea;
	public int currentArea;

	private void OnEnable()
	{
		GameEvents.only.enemyKilled += OnEnemyKilled;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyKilled -= OnEnemyKilled;
	}

	private void Start()
	{
		if(enemiesInArea.Count == 0)
		{
			Debug.LogWarning("Level Flow Controller values not changed");
			Debug.Break();
		}
		
		enemiesInCurrentArea = enemiesInArea[currentArea];
		enemiesKilledInCurrentArea = 0;
	}

	private void OnEnemyKilled()
	{
		enemiesKilledInCurrentArea++;
		if(enemiesKilledInCurrentArea >= enemiesInCurrentArea)
			MoveToNextArea();
	}

	private void MoveToNextArea()
	{
		if (currentArea == enemiesInArea.Count - 1)
		{
			print("game end");
			GameEvents.only.InvokeGameEnd();
		}
		else
		{
			enemiesInCurrentArea = enemiesInArea[++currentArea];
			enemiesKilledInCurrentArea = 0;
			GameEvents.only.InvokeNextArea();
			//assign disabled state, remove it at trigger
		}
	}
}
