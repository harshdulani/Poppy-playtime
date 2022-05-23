using UnityEngine;
using DG.Tweening;

public class StickmanMovementController : MonoBehaviour
{
	[SerializeField] private float fastMoveSpeedMultiplier = 1.2f, maxDistanceToPlayer,speed;
	
	private Animator _anim;
	
	private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int Attack = Animator.StringToHash("attack1");
	private static readonly int Speed = Animator.StringToHash("speed");
	
	
	private Transform _player, _transform;
	private float _currentSpeedMultiplier = 1f, _myMaxDistance;
	private bool _hasReached,_hasGameStarted;
	private bool hasWon;
	private bool isDead;

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer += OnGameLose;
		GameEvents.Only.PunchHit += GetHit;
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnEnemyReachPlayer()
	{
		ToggleAI(false);
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
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_anim = GetComponent<Animator>();
		
		_anim.SetFloat(Speed,speed);

		_currentSpeedMultiplier = fastMoveSpeedMultiplier;
		_myMaxDistance = Random.Range(maxDistanceToPlayer - 4f, maxDistanceToPlayer);
		print(isDead);
		print(hasWon);
	}

	private void Update()
	{
		if (isDead) return;
		if (hasWon) return;
		if (!_hasGameStarted) return;
		
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
	
	public void ToggleAI(bool status)
	{
		print("toggle AI: " + gameObject.name);
			_anim.SetBool(IsWalking, status);
	}
	
	private void OnTapToPlay()
	{
			
			DOVirtual.DelayedCall(Random.Range(0f, 0.25f), () =>
			{
				ToggleAI(true);
				_hasGameStarted = true;
			
			});
	
	}
}
