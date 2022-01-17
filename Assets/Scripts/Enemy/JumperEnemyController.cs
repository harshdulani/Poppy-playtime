using DG.Tweening;
using UnityEngine;

public class JumperEnemyController : MonoBehaviour
{
	[SerializeField] private Transform target;
	
	private Animator _anim;
	private static readonly int IsJumping = Animator.StringToHash("isJumping");
	private static readonly int IsMirrored = Animator.StringToHash("isMirrored");

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
	}
	
	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}

	public void OnJumpLanding()
	{
		var tempTarget = target.position;
		tempTarget.y = transform.position.y;
		
		transform.DORotateQuaternion(Quaternion.LookRotation(tempTarget - transform.position), .3f);
		_anim.SetBool(IsMirrored, Random.Range(0, 1f) > 0.5f);
	}
	
	private void OnTapToPlay()
	{
		_anim.SetBool(IsJumping, true);
	}
}