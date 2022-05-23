using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanMovementController : MonoBehaviour
{
	[SerializeField] private float fastMoveSpeedMultiplier = 1.2f, maxDistanceToPlayer;
	

	private Transform _player, _transform;
	private float _currentSpeedMultiplier = 1f, _myMaxDistance;
	private bool _hasReached;
	private bool hasWon;
	private bool isDead;

	private void OnEnable()
	{
		GameEvents.Only.EnemyKillPlayer += OnGameLose;
		GameEvents.Only.PunchHit += GetHit;
	}

	private void OnDisable()
	{
		GameEvents.Only.EnemyKillPlayer -= OnGameLose;
		GameEvents.Only.PunchHit -= GetHit;
	}

	private void Start()
	{
		

		_transform = transform;
		_player = GameObject.FindGameObjectWithTag("Player").transform;

		_currentSpeedMultiplier = fastMoveSpeedMultiplier;
		_myMaxDistance = Random.Range(maxDistanceToPlayer - 4f, maxDistanceToPlayer);
	}

	private void Update()
	{
		if (isDead) return;
		if (hasWon) return;

		_transform.position += Vector3.forward * (VehicleMovement.MovementSpeed * _currentSpeedMultiplier * Time.deltaTime);
			
		if(_hasReached) return;
		if(Vector3.Distance(_transform.position, _player.position) > _myMaxDistance) return;
		_currentSpeedMultiplier = 1f;
	}

	public void GetHit()
	{
		if(isDead) return;

		isDead = true;
		//GameEvents.Only.InvokeEnemyDied(_transform);
		
		
	}

	private void OnGameLose()
	{
		if(isDead) return;

		hasWon = true;
	}
}
