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
	[SerializeField] private float yTravelDistance = 1f;

	private PlayerSoundController _soundController;
	private Canvas _canvas;
	private Transform _transform;

	private Quaternion _areaInitRotation;
	private float _rotX, _rotY, _initRotAxisX, _initRotAxisY;
	private float _targetDistance, _targetInitYPos, _targetDesiredYPos;

	private float _currentYDistanceLerper;
	private float CurrentYDistanceLerper
	{
		get => _currentYDistanceLerper;
		/*{
			print("getting " + _currentYDistanceLerper);
			return _currentYDistanceLerper;
		}*/
		set => _currentYDistanceLerper = value;
	}
	private bool _canPlayLockOnSound = true;

	private Transform _myTarget;
	private Tweener _punchHit;
	private Tween _yPosTween;
	private bool _hasTarget;

	private void OnEnable()
	{
		GameEvents.Only.PunchHit += OnPunchHit;
		
		GameEvents.Only.TrapButtonPressed += OnTrapButtonPressed;
		GameEvents.Only.MoveToNextArea += OnMoveToNextArea;
		GameEvents.Only.ReachNextArea += OnReachNextArea;
	}

	private void OnDisable()
	{
		GameEvents.Only.PunchHit -= OnPunchHit;
		
		GameEvents.Only.TrapButtonPressed -= OnTrapButtonPressed;
		GameEvents.Only.MoveToNextArea -= OnMoveToNextArea;
		GameEvents.Only.ReachNextArea -= OnReachNextArea;
	}

	private void Start ()
	{
		_canvas = GameObject.FindGameObjectWithTag("AimCanvas").GetComponent<Canvas>();
		_soundController = GetComponent<PlayerSoundController>();
		
		_canvas.worldCamera = Camera.main;
		_transform = transform;

		_reticle = _canvas.transform.GetChild(0).GetComponent<Image>();

		_areaInitRotation = _transform.rotation;
		var rot = _areaInitRotation.eulerAngles;

		_initRotAxisX = rot.x;
		_initRotAxisY = rot.y;
		
		_rotY = rot.y;
		_rotX = rot.x;
	}

	public void Aim(Vector2 inputDelta)
	{
		_rotY += inputDelta.x * aimSpeedHorizontal * Time.deltaTime;
		_rotX -= inputDelta.y * aimSpeedVertical * Time.deltaTime;
 
		_rotY = Mathf.Clamp(_rotY, _initRotAxisY - clampAngleHorizontal, _initRotAxisY + clampAngleHorizontal);
		_rotX = Mathf.Clamp(_rotX, _initRotAxisX - clampAngleVertical, _initRotAxisX + clampAngleVertical);

		var newRot = Quaternion.Euler(_rotX, _rotY, 0.0f);
		_transform.rotation = newRot;
	}

	public void AimWithTargetHeld(Vector2 delta)
	{
		Aim(delta);
		
		ChangeTargetTransformation();
	}

	private void ChangeTargetTransformation()
	{
		//sometimes target is not set
		if (!_myTarget) return;
		if(HandController.PropHeldToPunch)
			if (HandController.PropHeldToPunch.isCar) return;
		
		var desiredPos = _transform.position + _transform.forward * _targetDistance;
		desiredPos.y = Mathf.Lerp(_targetInitYPos, _targetDesiredYPos, CurrentYDistanceLerper);
		
		_myTarget.position = Vector3.Lerp(_myTarget.position, desiredPos, CurrentYDistanceLerper);
		
		var direction = _transform.position - desiredPos;
		if (HandController.PropHeldToPunch) direction.y = 0;
		
		direction.y = 0f;
		_myTarget.rotation = Quaternion.LookRotation(direction);
		
		if (!HandController.PropHeldToPunch) _myTarget.rotation *= Quaternion.Euler(Vector3.left * 20f);
	}

	public void CalculateTargetDistance()
	{
		if(!HandController.TargetHeldToPunch) return;

		var targetPos = HandController.TargetHeldToPunch.position;
		_myTarget = HandController.TargetHeldToPunch;

		targetPos.y = _transform.position.y;
		_targetDistance = Vector3.Distance(_transform.position, targetPos);
		_targetInitYPos = _myTarget.position.y;
		
		if(HandController.PropHeldToPunch)
		{
			//is car condition is not being used because i tell the entry itself to be guard claused by if car get out
			if (HandController.PropHeldToPunch.isCar)
				_targetDesiredYPos = _targetInitYPos - yTravelDistance * -0.5f;
			else
				_targetDesiredYPos = _targetInitYPos - yTravelDistance * 0.5f;
		}
		else
			_targetDesiredYPos = _targetInitYPos - yTravelDistance;
	}

	public void MoveTargetDown()
	{
		if(_yPosTween.IsActive()) return;
		
		CurrentYDistanceLerper = 0f;
		_yPosTween = DOTween.To(() => CurrentYDistanceLerper, value => CurrentYDistanceLerper = value, 1f, .25f);
	}

	public void BringTargetBackUp()
	{
		if(_yPosTween.IsActive()) return;
		
		_yPosTween = DOTween.To(() => CurrentYDistanceLerper, value => CurrentYDistanceLerper = value, 0f, .25f)
			.OnUpdate(ChangeTargetTransformation).OnComplete(() => _myTarget = null);
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

		DOVirtual.DelayedCall(1f, ResetLockOnSound);
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

	private void ResetRotationToAreaInit()
	{
		_punchHit = _transform.DORotate(new Vector3(_initRotAxisX, _initRotAxisY, 0), 1f);
		_rotX = _initRotAxisX;
		_rotY = _initRotAxisY;
	}

	private void OnTrapButtonPressed()
	{
		ResetRotationToAreaInit();
	}

	private void OnPunchHit()
	{
		_punchHit = _transform.DORotate(new Vector3(_initRotAxisX, _initRotAxisY, 0), 1f);
		_rotX = _initRotAxisX;
		_rotY = _initRotAxisY;
	}

	private void OnMoveToNextArea()
	{
		_punchHit.Kill();
		_transform.DORotateQuaternion(_areaInitRotation, 0.5f);
		_rotX = _initRotAxisX;
		_rotY = 0f;
	}

	private void OnReachNextArea()
	{
		_areaInitRotation = _transform.rotation;
		var rot = _areaInitRotation.eulerAngles;

		_initRotAxisX = rot.x;
		_initRotAxisY = rot.y;
		
		_rotY = rot.y;
		_rotX = rot.x;
	}
}
