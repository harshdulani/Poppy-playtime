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
		if(!other.CompareTag("Target")) return;

		//if its an enemy who isnt waiting to be punched, punch me and kill me
		if(other.transform.TryGetComponent(out RagdollLimbController raghu) && !raghu.IsRaghuWaitingForPunch())
		{
			raghu.DisableRagdolling();

			raghu.Attack(transform.position + transform.root.forward.normalized * attackDistance);
			//if youre going to attack, knock out the guy whos waiting for punch/explode the prop
			if (_waitingForPunch)
				_waitingForPunch.GetPunched(Vector3.down, 5f);
			else if(_prop)
				if(_prop.TryGetComponent(out PropController prop))
					prop.Explode();
			
			return;
		}
		
		//if there is already

		GameEvents.only.InvokeEnterHitBox(other.transform);
		InputHandler.Only.WaitForPunch(other.transform);
		other.transform.TryGetComponent(out _waitingForPunch);
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