using DG.Tweening;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
	[SerializeField] private Transform boxingHandExtender;
	
	[ContextMenu("PressButton")]
	public void PressButton()
	{
		transform.DOLocalMoveY(transform.localPosition.y - 0.5f, 1f).SetLoops(2, LoopType.Yoyo);

		boxingHandExtender.DOScaleZ(3f, 0.75f).SetLoops(2, LoopType.Yoyo);
	}
}