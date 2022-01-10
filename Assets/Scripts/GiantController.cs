using DG.Tweening;
using UnityEngine;

public class GiantController : MonoBehaviour
{
	[HideInInspector] public bool isDead;

	[SerializeField] private Renderer rend;

	private Animator _anim;
	
	private static readonly int Hit = Animator.StringToHash("Hit");
	private static readonly int Jump = Animator.StringToHash("Jump");

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += ReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= ReachNextArea;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		rend.enabled = false;
	}

	public void StartJumpOnAnimation()
	{
		//this duration comes from animation - event keyframe time with anim fps, approx 32 * 1/60 of a second 
		transform.DOMoveY(0f, 0.5f).SetEase(Ease.InQuad);
	}

	public void ScreenShakeOnAnimation()
	{
		CameraController.only.ScreenShake(2f);
	}

	public void GetHit()
	{
		_anim.SetTrigger(Hit);
	}
	
	private void ReachNextArea()
	{
		if(!LevelFlowController.only.IsThisLastEnemy()) return;
		
		rend.enabled = true;
		_anim.SetTrigger(Jump);
	}
}