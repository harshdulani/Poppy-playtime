using UnityEngine;

public class StickmanBodyCollider : MonoBehaviour
{
	//private RagdollController raghu;
	private StickmanRagdollerController _raghu;
	private StickmanMovementController _stickmanMovement;

	private void Start()
	{
		_raghu = transform.root.GetComponent<StickmanRagdollerController>();
		_stickmanMovement = transform.root.GetComponent<StickmanMovementController>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(other.transform.root == transform.root) return;
		if (!other.transform.CompareTag("PropObject") && !other.transform.CompareTag("TrapButton")) return;
		
		GetHit();
		_stickmanMovement.EnableParticles();
		other.transform.tag = "Untagged";
	}

	public void GetHit()
	{
		_raghu.GoRagdoll(Vector3.back);
		_stickmanMovement.isDead = true;
	}
}
