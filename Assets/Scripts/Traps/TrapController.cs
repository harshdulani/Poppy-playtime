using UnityEngine;

public class TrapController : MonoBehaviour
{
	[SerializeField] private SerializableITrap myTrap;

	public void HandleTrapBehaviour() => myTrap.TrapBehaviour();
	public float GetTweenDuration() => myTrap.GetTweenDuration();
	public bool IsInCurrentArea() => myTrap.IsInCurrentArea();
}