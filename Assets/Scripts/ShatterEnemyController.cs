using UnityEngine;

public class ShatterEnemyController : MonoBehaviour
{
	[SerializeField] private Transform climbing;
	[SerializeField] private bool controlAnimator;
	private RagdollController _ragdollController;
	
	private bool _hasReached;
	
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
		_hasReached = true;
	}

	private void OnShatteredTower(Transform shattered)
	{
		if(shattered != climbing) return;
		
		_ragdollController.GoRagdoll(-transform.forward + transform.up * 6f);
		GameEvents.only.InvokeEnemyKill();
	}
}