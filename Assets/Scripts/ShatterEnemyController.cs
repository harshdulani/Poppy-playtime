using UnityEngine;

public class ShatterEnemyController : MonoBehaviour
{
	[SerializeField] private Transform climbing;
	[SerializeField] private bool controlAnimator;
	private RagdollController _ragdollController;
	
	[SerializeField] private int myPatrolArea;
	public bool shouldPatrol, hasClimbingTransform;
	
	private Animator _anim;
	private static readonly int Reached = Animator.StringToHash("reached");

	private int _currentWayPoint;
	private static readonly int StartClimbing = Animator.StringToHash("startClimbing");

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.reachNextArea += OnReachNextArea;
		GameEvents.only.rayfireShattered += OnShatteredTower;
	}
	
	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.reachNextArea -= OnReachNextArea;
		GameEvents.only.rayfireShattered -= OnShatteredTower;
	}

	private void Start()
	{
		if(controlAnimator)
		{
			_anim = GetComponent<Animator>();
			_anim.applyRootMotion = false;
		}
		_ragdollController = GetComponent<RagdollController>();

		if(!climbing)
			Debug.LogWarning("climber not assigned to " + gameObject);
	}
	
	public void ReachEnd()
	{
		if(controlAnimator)
			_anim.SetTrigger(Reached);
	}

	private void OnShatteredTower(Transform shattered)
	{
		if(shattered != climbing) return;
	
		_ragdollController.GoRagdoll(-transform.forward + transform.up * 6f);
		GameEvents.only.InvokeEnemyKill();
	}
	
	private void OnTapToPlay()
	{
		if(!shouldPatrol) return;
		if(myPatrolArea > 0) return;

		if (!controlAnimator) return;
		_anim.SetTrigger(StartClimbing);
		_anim.applyRootMotion = true;
	}
	
	private void OnReachNextArea()
	{
		if(!shouldPatrol) return;
		
		if(myPatrolArea != LevelFlowController.only.currentArea) return;
		
		if (!controlAnimator) return;
		_anim.SetTrigger(StartClimbing);
	}

	public void SetClimbingTransform(Transform newGuy)
	{
		if (!newGuy)
		{
			climbing = null;
			hasClimbingTransform = false;
			return;
		}

		climbing = newGuy;
		hasClimbingTransform = true;
	}
}