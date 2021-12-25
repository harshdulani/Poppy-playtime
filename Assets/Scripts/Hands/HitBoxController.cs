using UnityEngine;

public class HitBoxController : MonoBehaviour
{
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
		
		if(other.transform.root.TryGetComponent(out RagdollController raghu) && !raghu.isWaitingForPunch) return;
		
		print(1);
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