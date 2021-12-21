using UnityEngine;

public class SplineTriggerController : MonoBehaviour
{
	private MovementController _movementController;
	
	private void Start()
	{
		_movementController = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementController>();
	}

	public void StopFollowing()
	{
		_movementController.StopFollowing();
	}
}
