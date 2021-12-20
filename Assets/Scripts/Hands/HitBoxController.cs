using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
	private List<Collider> _targets;
	private bool _inHitBox;
	
	private bool _canRegisterEntry = true;

	private void Start()
	{
		_targets = new List<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(!_canRegisterEntry) return;
		
		if(!other.CompareTag("Target")) return;
		
		_targets.Add(other);
		
		if (_inHitBox) return;
		
		GameEvents.only.InvokeEnterHitBox(other.transform);
		InputHandler.Only.WaitForPunch(other.transform, transform.position.z);
		_inHitBox = true;
		_canRegisterEntry = false;
		Invoke(nameof(ResetRegisterable), 1f);
	}

	private void OnTriggerExit(Collider other)
	{
		if(!other.CompareTag("Target")) return;
		
		_targets.Remove(other);
		
		if (_targets.Count == 0)
			_inHitBox = false;
	}

	private void ResetRegisterable()
	{
		_canRegisterEntry = true;
	}
}
