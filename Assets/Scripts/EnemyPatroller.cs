using UnityEngine;
using UnityEngine.AI;

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

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();
		
		_anim.SetBool(IsWalking, shouldPatrol);
		SetNextWaypoint();
	}

	private void Update()
	{
		print(_agent.isStopped + ", " + _agent.hasPath);
		
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
		_agent.enabled = status;
	}
}
