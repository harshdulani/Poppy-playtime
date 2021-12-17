using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	private List<Collider> _targets;
	private bool _inHitBox;

	private void Start()
	{
		_targets = new List<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		_targets.Add(other);

		if (_inHitBox) return;
		
		GameEvents.only.InvokeEnterHitBox();
		print("Replace with private vars and Action");
		InputHandler.Only._rightHand.WaitForPunch(other.transform, transform.position.z);
		_inHitBox = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		_targets.Remove(other);
		
		if (_targets.Count == 0)
			_inHitBox = false;
	}
}
