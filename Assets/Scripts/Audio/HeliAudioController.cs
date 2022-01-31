using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeliAudioController : MonoBehaviour
{
	[SerializeField] private bool enableEngine;
	[SerializeField] private float idleVolume, combatVolume, minPitch, maxPitch, minVol, maxVol;

	private HelicopterController _heli;
	private AudioSource _audioSource;
	private float _initYPos;
	private bool _isIdle = true, _isDead;

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}
	
	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
		_heli = GetComponent<HelicopterController>();
		
		//playing in low volume since we are not using 3d audio
		//volume is increased to normal at Drive end

		_initYPos = transform.position.y;
		_audioSource.volume = idleVolume;

		OnReachNextArea();
	}

	private void Update()
	{
		if(_isIdle) return;
		if(_isDead) return;
		
		var diff = _heli.startTransform.position.y - _initYPos;

		//if you are at initpos, min pitch, if at 
		_audioSource.volume = Mathf.Lerp(minPitch, maxPitch, Mathf.InverseLerp(_initYPos, _heli.startTransform.position.y, transform.position.y)); 
	}

	private void OnReachNextArea()
	{
		if (!enableEngine) return;

		if (_heli.myArea != LevelFlowController.only.currentArea) return;
		
		_audioSource.volume = combatVolume;
		_isIdle = false;
	}


	public void OnHeliDeath()
	{
		_isDead = true;
		
		//volumes dotweens down
		DOTween.To(() => _audioSource.volume, value => _audioSource.volume = value, 0, 5f);

		//pitch dotweens a notch up quickly
		DOTween.To(() => _audioSource.pitch, value => _audioSource.pitch = value, _audioSource.pitch / 2f, 1f);
	}
}
