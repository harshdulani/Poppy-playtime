using UnityEngine;

public class ClimberController : MonoBehaviour
{
	private Animator _anim;
	private static readonly int Reached = Animator.StringToHash("reached");

	private void Start()
	{
		_anim = GetComponent<Animator>();
	}
	
	public void ReachEnd()
	{
		_anim.SetTrigger(Reached);
	}
}