using UnityEngine;

public abstract class SerializableATrap : MonoBehaviour
{
	[SerializeField] private int myArea;

	[ContextMenu("TrapBehaviour")] public abstract void TrapBehaviour();
	public abstract float GetTweenDuration();

	public bool IsInCurrentArea() => myArea == LevelFlowController.only.currentArea;
}