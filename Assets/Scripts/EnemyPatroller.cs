using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPatroller : MonoBehaviour
{
	[SerializeField] private Transform[] waypoints;
	public bool shouldPatrol;
	
	private NavMeshAgent _agent;
	private Animator _anim;

	private Vector3 _currentDest;
	private int CurrentWayPoint
	{
		get => _currentWayPoint;
		set => _currentWayPoint = value % waypoints.Length;
	}

	private int _currentWayPoint;
	
	private static readonly int IsWalking = Animator.StringToHash("isWalking");

	private void OnEnable()
	{
		GameEvents.only.enemyReachPlayer += OnEnemyReachPlayer;
	}

	private void OnDisable()
	{
		GameEvents.only.enemyReachPlayer -= OnEnemyReachPlayer;
	}

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		
		_anim.SetBool(IsWalking, shouldPatrol);
		SetNextWaypoint();
	}

	private void Update()
	{
		if(!shouldPatrol) return;
		
		if(Vector3.Distance(transform.position, _currentDest) > 0.5f) return;

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

	private void OnEnemyReachPlayer()
	{
		ToggleAI(false);
	}
}
