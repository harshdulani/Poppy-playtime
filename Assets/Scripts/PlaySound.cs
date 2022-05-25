using UnityEngine;

public class PlaySound : MonoBehaviour
{
	private bool _hasFallen;
	private void OnTriggerEnter(Collider other)
	{
		if(_hasFallen) return;
		if(!other.TryGetComponent(out RagdollLimbController _)) return;

		_hasFallen = true;
		AudioManager.instance.Play("BrickBreakHigh");
		AudioManager.instance.Play("BrickFall" + Random.Range(1, 3));
	}
}