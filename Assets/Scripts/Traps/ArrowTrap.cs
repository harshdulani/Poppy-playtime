using DG.Tweening;
using UnityEngine;

public class ArrowTrap : SerializableATrap
{
	[SerializeField] private Transform[] arrows;
	[SerializeField] private float arrowEndDistance, arrowFireDuration;
	[SerializeField] private bool returnToOrigin;

	public override void TrapBehaviour()
	{
		var seq = DOTween.Sequence();
		
		foreach (var arrow in arrows)
		{
			var startPos = arrow.position;
			seq.Join(arrow.DOMove(startPos- arrow.forward * arrowEndDistance, arrowFireDuration * 0.75f)
				.SetEase(Ease.OutBack).OnComplete(() =>
				{
					if (!returnToOrigin) return;
					
					arrow.position = startPos + arrow.forward * arrowEndDistance / 2f;
					arrow.DOMove(startPos, arrowFireDuration * 0.25f);
				}));
		}
	}

	public override float GetTweenDuration() => arrowFireDuration + .75f;
}