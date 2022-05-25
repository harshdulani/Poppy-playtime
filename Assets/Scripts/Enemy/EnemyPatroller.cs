using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatroller : MonoBehaviour
{
	public bool shouldPatrol;
	public int myPatrolArea;
	[SerializeField] private List<Transform> waypoints;
	[SerializeField] private bool hasWeapon,chasePlayer;
	[SerializeField] private float waypointChangeDistance = 0.5f, speed = 2f;

	private NavMeshAgent _agent;
	private Animator _anim;
	private EnemyChaseController _chaseController;

	private Vector3 _currentDest;
	private int CurrentWayPoint
	{
		get => _currentWayPoint;
		set => _currentWayPoint = value % waypoints.Count;
	}

	private int _currentWayPoint;

	private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int Attack = Animator.StringToHash("attack1");
	private static readonly int Speed = Animator.StringToHash("speed");

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.ReachNextArea += OnReachNextArea;
		
		GameEvents.Only.EnemyKillPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		GameEvents.Only.ReachNextArea -= OnReachNextArea;

		GameEvents.Only.EnemyKillPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		if(chasePlayer)
			_anim.SetFloat(Speed,speed);
		_anim.applyRootMotion = true;
		if(waypoints.Count == 0)
			waypoints.Add(GameObject.FindGameObjectWithTag("Player").transform);
		
	}

	private void Update()
	{
		if(!shouldPatrol) return;

		var distance = Vector3.Distance(transform.position, _currentDest); 
		
		if(distance > waypointChangeDistance) return;
		
		if (hasWeapon)
		{
			ToggleAI(false);
			print("I called toggle");
			_anim.SetTrigger(Attack);
			shouldPatrol = false;
			return;
		}

		if (waypoints.Count > 1)
			SetNextWaypoint();
		else
		{
			if(!chasePlayer)
				ToggleAI(false);
		}
			
	}

	private void SetNextWaypoint()
	{
		_currentDest = waypoints[CurrentWayPoint++].position;
		_agent.SetDestination(_currentDest);
	}

	public void ToggleAI(bool status)
	{
		if (!chasePlayer)
		{
			shouldPatrol = status;
			if(_agent)
				_agent.enabled = status;
			_anim.SetBool(IsWalking, status);
		}
		else
			_anim.SetBool(IsWalking, status);
	}

	private void OnTapToPlay()
	{
		if (!chasePlayer)
		{
			if(!shouldPatrol) return;
			if(myPatrolArea > 0) return;
			if(!chasePlayer)
				_anim.applyRootMotion = false;
			//this is perma true because uske bina soona soona lagta hai ghar
		
			DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
			{
				ToggleAI(true);
				_anim.SetBool(IsWalking, true);
				SetNextWaypoint();
			});
		}
		else
		{
			DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
			{
				ToggleAI(true);
				_anim.SetBool(IsWalking, true);
			
			});
		}

		
	}

	public bool IsInCurrentPatrolArea() => LevelFlowController.only.currentArea == myPatrolArea;
	
	private void OnEnemyReachPlayer()
	{
		ToggleAI(false);
	}

	private void OnReachNextArea()
	{
		if(!shouldPatrol) return;
		
		if(myPatrolArea != LevelFlowController.only.currentArea) return;
		
		SetNextWaypoint();
		ToggleAI(true);
	}
}
