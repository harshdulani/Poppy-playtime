using UnityEngine;

public class ShatterEnemyLimbController : MonoBehaviour
{
	private ShatterEnemyController _parent;

	private void Start()
	{
		_parent = transform.root.GetComponent<ShatterEnemyController>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("ClimbEnd")) return;

		_parent.ReachEnd();
	}
}
