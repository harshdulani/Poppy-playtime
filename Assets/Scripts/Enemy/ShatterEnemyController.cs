using UnityEngine;

public class ShatterEnemyController : MonoBehaviour
{
	[SerializeField] private Transform climbing;
	[SerializeField] private bool controlAnimator;
	[SerializeField] private int myArea;
	
	private RagdollController _ragdollController;
	private Animator _anim;
	
	private static readonly int Reached = Animator.StringToHash("reached");

	private void OnEnable()
	{
		GameEvents.only.rayfireShattered += OnShatteredTower;
	}
	
	private void OnDisable()
	{
		GameEvents.only.rayfireShattered -= OnShatteredTower;
	}

	private void Start()
	{
		if(controlAnimator)
			_anim = GetComponent<Animator>();
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
		
		_ragdollController.GoRagdoll(-transform.forward + transform.up * 2f);
	}
	
	public bool IsInCurrentArea() => LevelFlowController.only.currentArea == myArea;
}