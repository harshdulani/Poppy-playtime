using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAnalyticsSDK.Setup;
using UnityEngine;


	public class LevelFlowController : MonoBehaviour
	{
		public static LevelFlowController only;

		public bool isGiantLevel;
		[SerializeField] private List<int> enemiesInArea;
		[SerializeField] private List<Transform> throwTargetsInArea;

		public int enemiesInCurrentArea, enemiesKilledInCurrentArea;
		public int currentArea;
		private readonly List<Transform> _deadBodies = new List<Transform>();
		private int _totalEnemiesRemaining;

		private void OnEnable()
		{
			GameEvents.Only.EnemyKilled += OnEnemyKilled;
			GameEvents.Only.EnemyDied += OnEnemyDied;
			GameEvents.Only.ReachNextArea += OnReachNextArea;
		}

		private void OnDisable()
		{
			GameEvents.Only.EnemyKilled -= OnEnemyKilled;
			GameEvents.Only.EnemyDied -= OnEnemyDied;
			GameEvents.Only.ReachNextArea -= OnReachNextArea;
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

			if (throwTargetsInArea == null || throwTargetsInArea.Count == 0)
			{
				throwTargetsInArea = new List<Transform>();

				for (var i = 0; i < enemiesInArea.Count; i++)
					throwTargetsInArea.Add(null);
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
			//were waiting so that the Wrist/ WristController can return to arm

			const float timer = 1f;
			var elapsed = 0f;
			const float intervals = 0.2f;
			while (!InputHandler.IsInIdleState())
			{
				elapsed += intervals;
				yield return GameExtensions.GetWaiter(intervals);
				if (timer < elapsed) break;
			}

			while (HandController.IsWaitingToGivePunch)
				yield return GameExtensions.GetWaiter(intervals);

			if (currentArea == enemiesInArea.Count - 1)
				GameEvents.Only.InvokeGameEnd();
			else
			{
				GameEvents.Only.InvokeMoveToNextArea();
				InputHandler.AssignTemporaryDisabledState();
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

		public bool IsThisLastEnemy() => _totalEnemiesRemaining == 1;

		public bool IsThisLastEnemyOfArea()
		{
			return enemiesInArea[currentArea] - enemiesKilledInCurrentArea == 1;
		}

		public bool DidKillLastEnemyOfArea()
		{
			return enemiesInArea[currentArea] == enemiesKilledInCurrentArea;
		}
		private void OnEnemyDied(Transform t)
		{
			print("on enemy died: " + t.gameObject.name);
			if(_deadBodies.Contains(t.transform)) return;
			print("on enemy died");
			OnEnemyKilled();
			_deadBodies.Add(t.transform);
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
