using DG.Tweening;
using UnityEngine;

public class BoxingGloveTrap : SerializableITrap
{
	[SerializeField] private float tweenDuration;
	
	public override void TrapBehaviour() => transform.DOScaleZ(3f, tweenDuration / 2).SetLoops(2, LoopType.Yoyo);

	public override float GetTweenDuration() => tweenDuration;
}