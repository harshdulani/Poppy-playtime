using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	private bool _allowedToPunch;
	
	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		InputHandler.AssignNewState(InputHandler.Only.ReturnWaitingToPunch(), true);
		LevelFlowController.only.SlowDownTime();
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		LevelFlowController.only.RevertTime();
	}
}
