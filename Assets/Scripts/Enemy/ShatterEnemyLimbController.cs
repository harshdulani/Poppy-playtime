using UnityEngine;

public class ShatterEnemyLimbController : MonoBehaviour
{
	private ShatterEnemyController _climber;

	private void Start()
	{
		_climber = transform.root.GetComponent<ShatterEnemyController>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("ClimbEnd")) return;

		_climber.ReachEnd();
	}
}