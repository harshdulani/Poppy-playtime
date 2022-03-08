using DG.Tweening;
using UnityEngine;

public class TrapButtonController : MonoBehaviour
{
	[SerializeField] private Color onSelectColor;
	private TrapController[] _myTrapControllers;

	private Color _initColor;
	private MeshRenderer _renderer;
	private float _initLocalPosY;
	private bool _canPress = true;

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();
		_myTrapControllers = GetComponents<TrapController>();

		_initColor = _renderer.material.color;
		_initLocalPosY = transform.localPosition.y;
	}
	
	[ContextMenu("PressButton")]
	public void PressButton()
	{
		if(!_canPress) return;
		
		var seq = DOTween.Sequence();
		GameEvents.only.InvokeTrapButtonPressed();

		foreach (var trap in _myTrapControllers)
			seq.AppendCallback(() => trap.HandleTrapBehaviour());
		
		seq.AppendCallback(() => _canPress = false);
		
		seq.Append(transform.DOLocalMoveY(_initLocalPosY - 0.125f, .25f).SetEase(Ease.InExpo));
		seq.Join(_renderer.material.DOColor(onSelectColor, 0.5f).SetEase(Ease.InExpo));
		seq.AppendInterval(0.5f);
		seq.Append(transform.DOLocalMoveY(_initLocalPosY, 0.5f).SetEase(Ease.Linear));
		seq.Join(_renderer.material.DOColor(_initColor, 0.5f).SetEase(Ease.InExpo));

		//the duration shouldn't be an issue given you assign all multiple traps with same duration
		seq.InsertCallback(_myTrapControllers[0].GetTweenDuration(), () => _canPress = true);
	}

	//the area shouldn't be an issue given you assign all multiple traps with same area
	public bool IsInCurrentArea() => _myTrapControllers[0].IsInCurrentArea();
}