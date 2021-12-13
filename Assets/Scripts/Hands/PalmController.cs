using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;
	
	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		myHand.HandReachTarget(other.transform);
	}
}