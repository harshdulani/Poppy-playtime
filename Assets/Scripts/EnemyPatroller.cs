using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatroller : MonoBehaviour
{
	[SerializeField] private int myPatrolArea;
	[SerializeField] private List<Transform> waypoints;
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

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		
		if(waypoints.Count == 0)
			waypoints.Add(GameObject.FindGameObjectWithTag("Player").transform);
	}

	private void Update()
	{
		if(!shouldPatrol) return;

		var distance = Vector3.Distance(transform.position, _currentDest); 
		if(distance > 0.5f) return;

		if(waypoints.Count > 2)
			SetNextWaypoint();
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
		
		_anim.SetBool(IsWalking, shouldPatrol);
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
}
