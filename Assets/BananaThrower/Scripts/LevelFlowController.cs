using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BananaThrower
{
	public class LevelFlowController : MonoBehaviour
	{
		public static LevelFlowController only;
	
		[SerializeField] private List<int> enemiesInArea;

		public int enemiesInCurrentArea, enemiesKilledInCurrentArea;
		public int currentArea;
		public float maxRayDistance = 50f;
		public bool isBeingChased;
		private int _totalEnemiesRemaining;

		private readonly List<Transform> _deadBodies = new List<Transform>();

		private void OnEnable()
		{
			GameEvents.Only.EnemyDied += OnEnemyDied;
		}

		private void OnDisable()
		{
			GameEvents.Only.EnemyDied += OnEnemyDied;
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

		private void OnEnemyKilled()
		{
			enemiesKilledInCurrentArea++;
			_totalEnemiesRemaining--;
			if (enemiesKilledInCurrentArea >= enemiesInCurrentArea) 
				GameEvents.Only.InvokeGameEnd();
		}
		
		private void OnEnemyDied(Transform thrower)
		{
			if(_deadBodies.Contains(thrower.transform)) return;
		
			OnEnemyKilled();
			_deadBodies.Add(thrower.transform);
		}
	}
}