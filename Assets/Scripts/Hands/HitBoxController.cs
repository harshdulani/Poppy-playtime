using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	private bool _allowedToPunch;
	
	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		InputHandler.Only._rightHand.DeliverPunch(other.transform);
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Target")) return;
	}
}
