using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StickmanMovementController : MonoBehaviour
{
	[SerializeField] private ParticleSystem _splashParticle;
	[SerializeField] private float fastMoveSpeedMultiplier = 1.2f, maxDistanceToPlayer;
	
	private readonly List<Transform> _colliders = new List<Transform>();

	private Animator _anim;
	private Transform _player, _transform, _transformRoot;
	
	private float _currentSpeedMultiplier = 1f, _myMaxDistance;
	private bool _hasReached,_hasGameStarted, _slowedDownTemporarily;
	private bool _hasWon, _isDead;
	
	private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int Attack = Animator.StringToHash("attack1");
	
	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer += OnGameLose;
		GameEvents.Only.PunchHit += GetHit;
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer -= OnGameLose;
		GameEvents.Only.PunchHit -= GetHit;
		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_transform = transform;
		_transformRoot = _transform.root;

		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_anim = GetComponent<Animator>();
		
		_currentSpeedMultiplier = fastMoveSpeedMultiplier;
		_myMaxDistance = Random.Range(maxDistanceToPlayer - 4f, maxDistanceToPlayer);
	}

	private void Update()
	{
		if (_isDead) return;
		if (_hasWon) return;
		if (!_hasGameStarted) return;
		
		_transform.position += Vector3.forward * (VehicleMovement.MovementSpeed * _currentSpeedMultiplier * Time.deltaTime);
		
		if(_hasReached) return;
		if(_slowedDownTemporarily) return;
		
		if(Vector3.Distance(_transform.position, _player.position) > _myMaxDistance) return;
		_currentSpeedMultiplier = 1f;
	}

	public void GetHit()
	{
		if(_isDead) return;

		_isDead = true;
		//GameEvents.Only.InvokeEnemyDied(_transform);
	}

	private void OnGameLose()
	{
		if(_isDead) return;

		_hasWon = true;
	}

	public void ToggleAI(bool status) => _anim.SetBool(IsWalking, status);
	private bool IsSomeoneInFront() => _colliders.Count > 0;

	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		if(other.transform.root == _transformRoot) return;
		
		if (_colliders.Contains(other.transform.root)) return;
		
		_colliders.Add(other.transform.root);
		
		_currentSpeedMultiplier = 1f;
		_slowedDownTemporarily = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		if(other.transform.root == _transformRoot) return;
		if(!IsSomeoneInFront()) return;
		if (!_colliders.Contains(other.transform.root)) return;
		
		_colliders.Remove(other.transform.root);
		
		if(IsSomeoneInFront()) return;
		
		_currentSpeedMultiplier = fastMoveSpeedMultiplier;
		_slowedDownTemporarily = false;
	}

	private void OnTapToPlay()
	{
		DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
		{
			ToggleAI(true);
			_hasGameStarted = true;
		});
	}

	private void OnEnemyReachPlayer()
	{

		BananaThrower.LevelFlowController.only.TryAssignMinDistance(Vector3.Distance(_transform.position, _player.position), this);
		DOVirtual.DelayedCall(0.1f, () =>
		{
			if (BananaThrower.LevelFlowController.only.closest == this)
			{
				var position = _transform.position;
				var position1 = _player.position;
				var vehiclePosZ = _player.root.position.z;
				Vector3 dirToLookIn = position1 - position;
				print(Vector3.Distance(position,position1));
				Vector3 pos = transform.position;
				transform.DOMove(
					new Vector3(0,pos.y,vehiclePosZ - 6), 0.9f)
					.OnStart(() =>
					{
						Quaternion rotation=Quaternion.LookRotation(dirToLookIn,Vector3.up);
						transform.rotation = rotation;
					})
						.OnComplete(() =>
					{
						Quaternion rotation=Quaternion.LookRotation(Vector3.forward,Vector3.up);
						transform.rotation = rotation;
						ToggleAI(false);
						_anim.SetTrigger(Attack);
					});
			}
			else
			{
				ToggleAI(false);
			}

		});
	}

	public void EnableParticles()
	{
		_splashParticle.gameObject.SetActive(true);
		_splashParticle.Play();
	}
}
