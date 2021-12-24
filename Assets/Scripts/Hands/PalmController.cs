using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;
	
	public static Transform _lastPickedTarget;
	private bool _canBeAdopted = true;
	
	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		
		if(myHand.isLeftHand)
			GameEvents.only.propDestroyed += OnPropDestroyed;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		if(myHand.isLeftHand)
			GameEvents.only.propDestroyed -= OnPropDestroyed;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!_canBeAdopted) return;
		if(!other.CompareTag("Target")) return;
		
		//play sound
		if (!myHand.isLeftHand) return;
		
		print("on trigg trans " + GetCurrentTransform());
		if(HasTargetTransform()) return;
		
		SetCurrentTransform(other.transform);
		myHand.HandReachTarget(other.transform);
		_canBeAdopted = false;
	}

	private void EnablePunching()
	{
		myHand.StopPunching();
	}

	private void OnPunchHit()
	{
		myHand.HandReachTarget(GetCurrentTransform());
		
		SetCurrentTransform(null);
		Invoke(nameof(EnablePunching), punchWaitTime);
		Invoke(nameof(ResetAdoptability), punchWaitTime);
	}

	private void OnPropDestroyed(Transform target)
	{
		if(target != _lastPickedTarget) return;
		
		SetCurrentTransform(null);
		Invoke(nameof(ResetAdoptability), punchWaitTime);
	}

	private void ResetAdoptability()
	{
		_canBeAdopted = true;
	}

	private static bool HasTargetTransform() => _lastPickedTarget;
	private static Transform GetCurrentTransform() => _lastPickedTarget;
	private static void SetCurrentTransform(Transform newT) => _lastPickedTarget = newT;
}