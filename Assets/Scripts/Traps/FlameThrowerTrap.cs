using DG.Tweening;
using UnityEngine;

public class FlameThrowerTrap : SerializableATrap
{
	[SerializeField] private GameObject[] flamethrowers;
	private bool _throwingFlame;

	private void OnDisable()
	{
		if(_throwingFlame)
			GameEvents.only.moveToNextArea -= OnMoveNextArea;
	}

	public override void TrapBehaviour()
	{
		foreach (var flame in flamethrowers)
			flame.SetActive(true);

		GameEvents.only.moveToNextArea += OnMoveNextArea;
		_throwingFlame = true;
	}

	public override float GetTweenDuration() => 0;

	private void OnTriggerEnter(Collider other)
	{
		if(!_throwingFlame) return;
		if(!other.CompareTag("Target")) return;

		foreach (var rend in other.transform.root.GetComponentsInChildren<Renderer>()) 
			rend.material.DOColor(Color.black, 2.5f);

		if (other.TryGetComponent(out RagdollLimbController raghu)) 
			raghu.GetPunched(transform.position - other.transform.position, 1f);
	}
	
	private void OnMoveNextArea()
	{
		if(!_throwingFlame) return;
		
		foreach (var flame in flamethrowers)
			flame.SetActive(false);

		_throwingFlame = false;
	}
}