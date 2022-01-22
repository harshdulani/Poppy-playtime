using UnityEngine;

public class ShatterEnemyController : MonoBehaviour
{
	[SerializeField] private Transform climbing;
	[SerializeField] private bool controlAnimator, shouldPatrol;
	[SerializeField] private int myArea;
	
	private bool _hasCrossed;

	private EnemyPatroller _patroller;
	private Animator _anim;
	
	private static readonly int Reached = Animator.StringToHash("reached");
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
		_patroller = GetComponent<EnemyPatroller>();
	}
	
	public void ReachEnd()
	{
		if(controlAnimator)
			_anim.SetTrigger(Reached);

		_hasCrossed = true;
	}

	public bool IsInCurrentArea() => LevelFlowController.only.currentArea == myArea;

	private void OnShatteredTower(Transform shattered)
	{
		if(shattered != climbing) return;
		
		if(!_hasCrossed)
			if(_patroller)
				_patroller.ToggleAI(false);
	}
	
	private void OnTapToPlay()
	{
		if(!shouldPatrol) return;
		if(myArea > 0) return;

		if (!controlAnimator) return;
		
		_anim.SetTrigger(StartClimbing);
		_anim.applyRootMotion = true;
	}
	
	private void OnReachNextArea()
	{
		if(!shouldPatrol) return;
		if(!IsInCurrentArea()) return;
		
		if (!controlAnimator) return;
		_anim.SetTrigger(StartClimbing);
		_anim.applyRootMotion = true;
	}
}