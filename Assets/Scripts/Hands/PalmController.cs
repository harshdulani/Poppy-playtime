using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;

	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		//play sound
		if (!myHand.isLeftHand) return;
		
		myHand.HandReachTarget(other.transform);
	}

	private void EnablePunching()
	{
		myHand.StopPunching();
	}

	private void OnPunchHit()
	{
		myHand.HandReachTarget(InputHandler.Only.GetCurrentTransform());
		
		Invoke(nameof(EnablePunching), punchWaitTime);
	}
}