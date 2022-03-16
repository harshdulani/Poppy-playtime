using DG.Tweening;
using UnityEngine;

public class TrapController : MonoBehaviour
{
	[SerializeField] private SerializableATrap myTrap;
	[SerializeField] private float yTravelOnEnemyCollision;

	public void HandleTrapBehaviour() => myTrap.TrapBehaviour();
	public float GetTweenDuration() => myTrap.GetTweenDuration();
	public bool IsInCurrentArea() => myTrap.IsInCurrentArea();

	private void OnCollisionEnter(Collision other)
	{
		if(!other.collider.CompareTag("Target")) return;
		
		//if an enemy touches me, hide
		transform.DOMoveY(transform.position.y + yTravelOnEnemyCollision, 1f);
	}
}