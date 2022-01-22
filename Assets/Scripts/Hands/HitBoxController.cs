using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	[SerializeField] private float attackDistance;
	private RagdollLimbController _waitingForPunch;
	private Transform _prop;
	private bool _inTransit;

	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		GameEvents.only.propDestroyed += OnPropDestroyed;

		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		GameEvents.only.propDestroyed -= OnPropDestroyed;
		
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_inTransit) return;
		if(_prop) return;
		if(!other.CompareTag("Target")) return;

		//if its another enemy who isnt waiting to be punched, punch me and kill me
		if(other.transform.TryGetComponent(out RagdollLimbController raghu) && !raghu.IsRaghuWaitingForPunch())
		{
			raghu.DisableRagdolling();

			raghu.Attack(transform.position + transform.root.forward.normalized * attackDistance);
			
			//if youre going to attack, knock out the guy whos waiting for punch/explode the prop
			if (_waitingForPunch)
				_waitingForPunch.GetPunched(Vector3.down, 5f);

			if (!_prop) return;
			if (!_prop.TryGetComponent(out PropController prop)) return;
			
			prop.Explode();

			return;
		}

		_prop = other.transform;
		_prop.TryGetComponent(out _waitingForPunch);
		
		GameEvents.only.InvokeEnterHitBox(_prop);
		InputHandler.Only.WaitForPunch(_prop);
	}

	private void ResetInHitBox()
	{
		_waitingForPunch = null;
		_prop = null;
	}
	
	private void OnPunchHit()
	{
		Invoke(nameof(ResetInHitBox), 1f);
	}

	private void OnPropDestroyed(Transform destroyed)
	{
		if(destroyed == _prop)
			ResetInHitBox();
	}
	
	private void OnMoveToNextArea()
	{
		_inTransit = true;
	}

	private void OnReachNextArea()
	{
		_inTransit = false;
	}
}