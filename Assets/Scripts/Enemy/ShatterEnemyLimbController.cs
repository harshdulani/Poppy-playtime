using UnityEngine;

public class ShatterEnemyLimbController : MonoBehaviour
{
	[SerializeField] private bool shouldCheckForFloor;
	private ShatterEnemyController _parent;

	private void Start()
	{
		_parent = transform.root.GetComponent<ShatterEnemyController>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(other.collider.CompareTag(gameObject.tag)) return;

		if (_parent.hasClimbingTransform) return;
		
		if(shouldCheckForFloor)
			if (other.gameObject.TryGetComponent(out Shatterable shatterable))
				_parent.SetClimbingTransform(shatterable.GetShatterableParent().transform);
		
		if(!other.collider.CompareTag("ClimbEnd")) return;

		_parent.ReachEnd();
	}
	
	private void OnCollisionExit(Collision other)
	{
		if (!shouldCheckForFloor) return;

		if(other.collider.CompareTag(gameObject.tag)) return;

		if (other.gameObject.TryGetComponent(out Shatterable shatterable))
			_parent.SetClimbingTransform(null);
	}
}
