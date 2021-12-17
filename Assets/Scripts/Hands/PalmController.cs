using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PalmController : MonoBehaviour
{
	[SerializeField] private HandController myHand;

	[SerializeField] private float punchWaitTime = 1f;

	private bool _canPunch = true;

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
		_canPunch = true;
		myHand.StopPunching();
	}

	private void OnPunchHit()
	{
		myHand.HandReachTarget(InputHandler.Only.GetCurrentTransform());
		
		_canPunch = false;
		Invoke(nameof(EnablePunching), punchWaitTime);
	}
}