using UnityEngine;

public class WristController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Arm")) return;
	
		myHand.HandReachHome();
	}
}
