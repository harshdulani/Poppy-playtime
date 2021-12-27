using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	[SerializeField] private float attackDistance, inTransitTime = 2f;
	private bool _inHitBox, _inTransit;
	
	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.moveToNextArea += MoveToNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.moveToNextArea += MoveToNextArea;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_inTransit) return;
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
	
	
	private void MoveToNextArea()
	{
		_inTransit = true;
		Invoke(nameof(DisableTransit), inTransitTime);
	}

	private void DisableTransit()
	{
		_inTransit = false;
	}
}