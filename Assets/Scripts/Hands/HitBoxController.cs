using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	[SerializeField] private float attackDistance;
	private bool _inHitBox;
	
	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_inHitBox) return;
		
		if(!other.CompareTag("Target")) return;
		
		if(other.transform.TryGetComponent(out RagdollLimbController raghu) && !raghu.IsRaghuWaitingForPunch())
		{
			raghu.Attack(transform.position + transform.root.forward.normalized * attackDistance);
			return;
		}
		
		GameEvents.only.InvokeEnterHitBox(other.transform);
		InputHandler.Only.WaitForPunch(other.transform);
		_inHitBox = true;
	}

	private void ResetInHitBox()
	{
		_inHitBox = false;
	}
	
	private void OnPunchHit()
	{
		Invoke(nameof(ResetInHitBox), 1f);
	}
}