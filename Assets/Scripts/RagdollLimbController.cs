using DG.Tweening;
using UnityEngine;

public class RagdollLimbController : MonoBehaviour
{
	private RagdollController _parent;
	private HostageController _hostage;

	private Rigidbody _rb;
	
	private void Start()
	{
		if (!transform.root.TryGetComponent(out _parent))
			_hostage = transform.root.GetComponent<HostageController>();
		
		_rb = GetComponent<Rigidbody>();
	}

	public void TellParent()
	{
		_parent.HoldInAir();
	}
	
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
			GetPunched(-direction, direction.magnitude);
			return;
		}
		if (other.gameObject.TryGetComponent(out RagdollLimbController raghu) && !raghu._parent.isWaitingForPunch)
			raghu.GetPunched(direction, direction.magnitude);
		else
		{
			var prop = other.gameObject.GetComponent<PropController>();
			if(prop.shouldExplode)
				prop.Explode();
		}
	}
}
