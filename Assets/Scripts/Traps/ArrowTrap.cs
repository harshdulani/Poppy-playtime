using DG.Tweening;
using UnityEngine;

public class ArrowTrap : SerializableATrap
{
	[SerializeField] private TrapAttacker attacker;
	[SerializeField] private Transform[] arrows;
	[SerializeField] private float arrowEndDistance, arrowFireDuration;
	[SerializeField] private bool returnToOrigin;

	public override void TrapBehaviour()
	{
		var seq = DOTween.Sequence();

		attacker.isEnabled = true;
		foreach (var arrow in arrows)
		{
			var startPos = arrow.position;
			seq.Join(arrow.DOMove(startPos - arrow.forward * arrowEndDistance, arrowFireDuration * 0.85f)
				.SetEase(Ease.OutBack).OnComplete(() =>
				{
					if (!returnToOrigin)
					{
						attacker.isEnabled = false;
						return;
					}
					
					arrow.position = startPos + arrow.forward * arrowEndDistance / 2f;
					arrow.DOMove(startPos, arrowFireDuration * 0.15f).OnComplete(() => attacker.isEnabled = false);
				}));
		}
	}

	public override float GetTweenDuration() => arrowFireDuration + .75f;
}