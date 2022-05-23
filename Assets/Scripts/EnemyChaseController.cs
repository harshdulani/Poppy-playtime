using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyChaseController : MonoBehaviour
{
    public bool shouldPatrol;
	public int myPatrolArea;
	[SerializeField] private List<Transform> waypoints;
	[SerializeField] private bool chasePlayer;
	[SerializeField] private float speed = 2f;

	
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
		
		_anim = GetComponent<Animator>();
		if(chasePlayer)
			_anim.SetFloat(Speed,speed);
		_anim.applyRootMotion = true;
		if(waypoints.Count == 0)
			waypoints.Add(GameObject.FindGameObjectWithTag("Player").transform);
	}

	private void Update()
	{
		
			
	}

	private void SetNextWaypoint()
	{
		_currentDest = waypoints[CurrentWayPoint++].position;
		
	}

	public void ToggleAI(bool status)
	{
		print("status: " + status);
		_anim.SetBool(IsWalking, status);
	}

	private void OnTapToPlay()
	{
		
		if(!chasePlayer)
			_anim.applyRootMotion = false;
		//this is perma true because uske bina soona soona lagta hai ghar
		
		DOVirtual.DelayedCall(Random.Range(0f, 0.5f), () =>
		{
			ToggleAI(true);
			_anim.SetBool(IsWalking, true);
			
		});
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
