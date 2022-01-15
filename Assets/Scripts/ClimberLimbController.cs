using UnityEngine;

public class ClimberLimbController : MonoBehaviour
{
	private ClimberController _climber;

	private void Start()
	{
		_climber = transform.root.GetComponent<ClimberController>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("ClimbEnd")) return;

		_climber.ReachEnd();
	}
}