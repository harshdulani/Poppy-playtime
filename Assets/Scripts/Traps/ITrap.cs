using UnityEngine;

public interface ITrap
{
	public void TrapBehaviour();

	public float GetTweenDuration();
}

public abstract class SerializableITrap : MonoBehaviour, ITrap
{
	public abstract void TrapBehaviour();

	public abstract float GetTweenDuration();
}