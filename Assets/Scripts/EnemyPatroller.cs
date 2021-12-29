using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatroller : MonoBehaviour
{
	[SerializeField] private bool hasWeapon;
	[SerializeField] private int myPatrolArea;
	[SerializeField] private List<Transform> waypoints;
	[SerializeField] private float waypointChangeDistance = 0.5f;
	public bool shouldPatrol;
	
	private NavMeshAgent _agent;
	private Animator _anim;

	private Vector3 _currentDest;
	private int CurrentWayPoint
	{
		get => _currentWayPoint;
		set => _currentWayPoint = value % waypoints.Count;
	}

	private int _currentWayPoint;

	private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int Attack = Animator.StringToHash("attack1");

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		GameEvents.only.reachNextArea += OnReachNextArea;
		
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.reachNextArea -= OnReachNextArea;
		
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();

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
			_anim.SetTrigger(Attack);
			GameEvents.only.InvokeEnemyReachPlayer();
			shouldPatrol = false;
			return;
		}

		if (waypoints.Count > 1)
			SetNextWaypoint();
		else
		{
			shouldPatrol = false;
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
		shouldPatrol = status;
		_agent.enabled = status;
		_anim.SetBool(IsWalking, status);
	}

	private void OnTapToPlay()
	{
		if(myPatrolArea > 0) return;

		_anim.applyRootMotion = false;
		_anim.SetBool(IsWalking, shouldPatrol);
		ToggleAI(true);
		SetNextWaypoint();
	}

	private void OnEnemyReachPlayer()
	{
		ToggleAI(false);
	}

	private void OnMoveToNextArea()
	{
		if(myPatrolArea != LevelFlowController.only.currentArea) return;
		SetNextWaypoint();
		ToggleAI(true);
	}

	private void OnReachNextArea()
	{
		
	}
}
