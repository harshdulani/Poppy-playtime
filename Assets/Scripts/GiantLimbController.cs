using UnityEngine;

public class GiantLimbController : MonoBehaviour
{
	private GiantController _parent;

	private void Start()
	{
		_parent = transform.root.GetComponent<GiantController>();
	}
	
	private void OnCollisionEnter(Collision other)
	{
		if(!_parent) return;
	
		if(_parent.isDead) return;

		if(other.transform.root == transform.root) return;
		if (!other.collider.CompareTag("Target")) return;

		_parent.GetHit(other.transform);
	}
}