using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
	[SerializeField] private float aimSpeedVertical, aimSpeedHorizontal, clampAngleVertical, clampAngleHorizontal;
	[SerializeField] private Color findTargetColor, missingTargetColor;
	private Image _reticle;
	
	//the magic number percentage 0.5905f is the screen Y pos when you center the crosshair on anchorY as minY = 0.565, maxY = 0.615
	//0.5899 for 0.55, 0.63
	[Header("Aiming")] public float screenPercentageOnY = 0.5899f;

	private PlayerSoundController _soundController;
	private Canvas _canvas;

	private Quaternion _areaInitRotation;
	private float _rotX, _rotY, _initRotAxisX, _initRotAxisY;
	private bool _canPlayLockOnSound = true;
	
	private Tweener _punchHit;
	private bool _hasTarget;

	private void OnEnable()
	{
		GameEvents.only.punchHit += OnPunchHit;
		
		GameEvents.only.trapButtonPressed += OnTrapButtonPressed;
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		GameEvents.only.reachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.only.punchHit -= OnPunchHit;
		
		GameEvents.only.trapButtonPressed -= OnTrapButtonPressed;
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.reachNextArea -= OnReachNextArea;
	}

	private void Start ()
	{
		_canvas = GameObject.FindGameObjectWithTag("AimCanvas").GetComponent<Canvas>();
		_soundController = GetComponent<PlayerSoundController>();
		
		_canvas.worldCamera = Camera.main;

		_reticle = _canvas.transform.GetChild(0).GetComponent<Image>();

		_areaInitRotation = transform.rotation;
		var rot = _areaInitRotation.eulerAngles;

		_initRotAxisX = rot.x;
		_initRotAxisY = rot.y;
		
		_rotY = rot.y;
		_rotX = rot.x;
	}

	private void Update()
	{
		//print(InputExtensions.GetInputPosition().y / Screen.height);
	}

	public void SetReticleStatus(bool isOn)
	{
		_reticle.enabled = isOn;
	}

	public void FindTarget()
    {
	    _reticle.color = findTargetColor;
		if(!_hasTarget)
		{
			Vibration.Vibrate(10);
			_hasTarget = true;
		}
		
		if(!_canPlayLockOnSound) return;
		_soundController.PlaySound(_soundController.findTarget, .6f);
		_canPlayLockOnSound = false;
		Invoke(nameof(ResetLockOnSound), 1f);
	}

    public void LoseTarget()
    {
	    _reticle.color = missingTargetColor;
		_hasTarget = false;
	}

	private void ResetLockOnSound()
	{
		_canPlayLockOnSound = true;
	}
	
    public void Aim(Vector2 inputDelta)
    {
		_rotY += inputDelta.x * aimSpeedHorizontal * Time.deltaTime;
		_rotX -= inputDelta.y * aimSpeedVertical * Time.deltaTime;
 
		_rotY = Mathf.Clamp(_rotY, _initRotAxisY - clampAngleHorizontal, _initRotAxisY + clampAngleHorizontal);
		_rotX = Mathf.Clamp(_rotX, _initRotAxisX - clampAngleVertical, _initRotAxisX + clampAngleVertical);

		var newRot = Quaternion.Euler(_rotX, _rotY, 0.0f);
		transform.rotation = newRot;
	}

	private void ResetRotationToAreaInit()
	{
		_punchHit = transform.DORotate(new Vector3(_initRotAxisX, _initRotAxisY, 0), 1f);
		_rotX = _initRotAxisX;
		_rotY = _initRotAxisY;
	}

	private void OnTrapButtonPressed()
	{
		ResetRotationToAreaInit();
	}
	
	private void OnPunchHit()
	{
		_punchHit = transform.DORotate(new Vector3(_initRotAxisX, _initRotAxisY, 0), 1f);
		_rotX = _initRotAxisX;
		_rotY = _initRotAxisY;
	}
	
	private void OnMoveToNextArea()
	{
		_punchHit.Kill();
		transform.DORotateQuaternion(_areaInitRotation, 0.5f);
		_rotX = _initRotAxisX;
		_rotY = 0f;
	}

	private void OnReachNextArea()
	{
		_areaInitRotation = transform.rotation;
		var rot = _areaInitRotation.eulerAngles;

		_initRotAxisX = rot.x;
		_initRotAxisY = rot.y;
		
		_rotY = rot.y;
		_rotX = rot.x;
	}
}
