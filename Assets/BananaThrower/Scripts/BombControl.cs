using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class BombControl : MonoBehaviour
{
	[SerializeField] private bool drawDebugSphere, onDisplay;

	public bool isHeld;
	[Header("Explosion"), SerializeField] private GameObject particleFx;
	[SerializeField] private GameObject sparks, text;
	[SerializeField] private float explosionRadius;
	private bool _hasExploded;

	[Header("Animation"), SerializeField] private float bombHoverDuration;
	[SerializeField] private float bombHoverDelta;

	private Tween _hoverTween;
	
	private readonly Collider[] _overlaps = new Collider[100];
	private readonly List<Transform> _affected = new List<Transform>();

	private Transform _transform;
	private PlayerRefBank _player;

	private void Start()
	{
		_transform = transform;
		_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRefBank>();
		
		_transform.localScale = Vector3.one * 0.2f;

		if(onDisplay)
			_hoverTween = _transform.parent.DOLocalMoveY(_transform.position.y + bombHoverDelta, bombHoverDuration).SetLoops(-1, LoopType.Yoyo);
		else
			_transform.DOScale(Vector3.one * 0.5f, 0.5f);
	}

	private void OnDrawGizmos()
	{
		if(!drawDebugSphere) return;
		Gizmos.color = new Color(1f, 0f, 0f, 0.44f);
		Gizmos.DrawSphere(transform.position, explosionRadius);
	}

	private void Explode()
	{
		if(AudioManager.instance)
			AudioManager.instance.Play("BananaBomb");
		
		particleFx.SetActive(true);
		sparks.SetActive(false);
		particleFx.transform.parent = null;

		var size = Physics.OverlapSphereNonAlloc(_transform.position, explosionRadius, _overlaps);

		if(size == 0) return;
		
		for (var i = 0; i < size; i++)
		{
			var collu = _overlaps[i];
			if (!collu.CompareTag("Target")) continue;
			if(_affected.Contains(collu.transform.root)) continue;
			if(!collu.TryGetComponent(out StickmanBodyCollider body)) continue;

			body.GetHit();
			
			_affected.Add(collu.transform.root);
		}

		gameObject.SetActive(false);
	}

	public void HoldBomb()
	{
		if(!onDisplay) return;

		onDisplay = false;
		_player.Thrower.ThrowBombNextTime();
		_transform.parent = _player.bombHolder;
		DOTween.Kill(_transform);
		_hoverTween.Kill();
		_transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InBack);
		_transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InBack);
		_transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack);
		text.SetActive(false);
		isHeld = true;
	}
	
	public void HoldBomb(Transform hand)
	{
		if(!onDisplay) return;

		onDisplay = false;
		_transform.parent = hand;
		DOTween.Kill(_transform);
		_hoverTween.Kill();
		_transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.InBack);
		_transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.InBack);
		_transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack);
		text.SetActive(false);
		isHeld = true;
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_hasExploded) return;
		if (!other.collider.CompareTag("Ground") && !other.collider.CompareTag("Target")) return;

		_transform.tag = "Untagged";
		_hasExploded = true;
		Explode();
	}
}