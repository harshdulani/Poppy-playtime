using DG.Tweening;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
	[SerializeField] private Transform boxingHandExtender;
	[SerializeField] private Color onSelectColor;

	private Color _initColor;
	private MeshRenderer _renderer;
	private float _initLocalPosY;
	private bool _canPress = true;

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();

		_initColor = _renderer.material.color;
		_initLocalPosY = transform.localPosition.y;
	}
	
	[ContextMenu("PressButton")]
	public void PressButton()
	{
		if(!_canPress) return;
		var seq = DOTween.Sequence();

		seq.AppendCallback(() => _canPress = false);
		seq.Append(transform.DOLocalMoveY(_initLocalPosY - 0.125f, .25f).SetEase(Ease.InExpo));
		seq.Join(_renderer.material.DOColor(onSelectColor, 0.5f).SetEase(Ease.InExpo));
		seq.AppendInterval(0.5f);
		seq.Append(transform.DOLocalMoveY(_initLocalPosY, 0.5f).SetEase(Ease.Linear));
		seq.Join(_renderer.material.DOColor(_initColor, 0.5f).SetEase(Ease.InExpo));

		seq.Insert(0f, boxingHandExtender.DOScaleZ(3f, 0.75f).SetLoops(2, LoopType.Yoyo).OnComplete(() => _canPress = true));
	}
}