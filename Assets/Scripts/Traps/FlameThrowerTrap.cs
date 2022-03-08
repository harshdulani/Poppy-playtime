using DG.Tweening;
using UnityEngine;

public class FlameThrowerTrap : SerializableATrap
{
	[SerializeField] private GameObject[] flamethrowers;
	[SerializeField] private float trapDuration;
	private bool _throwingFlame;

	public override void TrapBehaviour()
	{
		foreach (var flame in flamethrowers)
			flame.SetActive(true);
		_throwingFlame = true;

		DOVirtual.DelayedCall(trapDuration, () =>
		{
			foreach (var flame in flamethrowers)
				flame.SetActive(false);
			
			_throwingFlame = false;
		}); 
	}

	public override float GetTweenDuration() => trapDuration + 0.75f;

	private void OnTriggerEnter(Collider other)
	{
		if(!_throwingFlame) return;
		if(!other.CompareTag("Target")) return;

		foreach (var rend in other.transform.root.GetComponentsInChildren<Renderer>()) 
			rend.material.DOColor(Color.black, 2.5f);

		if (other.TryGetComponent(out RagdollLimbController raghu)) 
			raghu.GetPunched(transform.position - other.transform.position, 1f);
	}
}