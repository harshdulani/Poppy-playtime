using DG.Tweening;
using UnityEngine;

public class RagdollLimbController : MonoBehaviour
{
	private RagdollController _parent;
	private ShatterEnemyController _shatterParent;
	private HostageController _hostage;
	
	private Rigidbody _rb;
	
	private void Start()
	{
		if (!transform.root.TryGetComponent(out _parent))
			_hostage = transform.root.GetComponent<HostageController>();

		transform.root.TryGetComponent(out _shatterParent);
		_rb = GetComponent<Rigidbody>();
	}

	public bool TellParent()
	{
		if (!_parent.TryHoldInAir()) return false;
		
		if (_shatterParent)
			_shatterParent.HoldInAir();
		return true;
	}

	public void DisableRagdolling() => _parent.isAttackerSoCantRagdoll = true;
	
	public void GetPunched(Vector3 direction, float punchForce)
	{
		if (_parent)
			_parent.GoRagdoll(direction);
		else
			_hostage.GoRagdoll(direction);
		_rb.AddForce(direction * punchForce + Vector3.up * punchForce / 3, ForceMode.Impulse);
	}

	
	public Rigidbody AskParentForHook() => _parent.chest;
	
	public bool IsRaghuWaitingForPunch() => _parent.isWaitingForPunch;

	public void Attack(Vector3 endPos)
	{
		if (_parent.isRagdoll) return;

		endPos.y = transform.root.position.y;
		_parent.transform.DOMove(endPos, 0.5f);
		_parent.AttackEnemy();
		GameEvents.only.InvokeEnemyKillPlayer();
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if(!_parent) return;
	
		if(!_parent.isRagdoll) return;

		if(other.transform.root == transform.root) return;
		if (!other.collider.CompareTag("Target")) return;
		
		var direction = other.transform.position - transform.position;
		if (_rb.velocity.sqrMagnitude < 4f)
		{
			if(_parent.isAttackerSoCantRagdoll) return;
			GetPunched(-direction, direction.magnitude);
			return;
		}
		if (other.gameObject.TryGetComponent(out RagdollLimbController raghu) && !raghu._parent.isWaitingForPunch)
			raghu.GetPunched(direction, direction.magnitude);
		else
		{
			var prop = other.gameObject.GetComponent<PropController>();
			if (!prop) return;
			
			if(prop.IsACompositeProp)
				prop.GetTouchedComposite(prop.transform.position - transform.position, true);
			
			if(prop.shouldExplode)
				prop.Explode();
		}
	}
}
