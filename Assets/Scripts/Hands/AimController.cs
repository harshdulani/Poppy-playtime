using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
	[SerializeField] private float aimSpeedVertical, aimSpeedHorizontal, clampAngleVertical, clampAngleHorizontal;
	[SerializeField] private Color findTargetColor, missingTargetColor;
	[SerializeField] private Image reticle;
	
	//the magic number percentage 0.5905f is the screen Y pos when you center the crosshair on anchorY as minY = 0.565, maxY = 0.615
	//0.5899 for 0.55, 0.63
	[Header("Aiming")] public float screenPercentageOnY = 0.5899f;


	private float _rotX, _rotY, _initRotAxisX;

	private Tweener _punchHit;
	private void OnEnable()
	{
		GameEvents.only.moveToNextArea += OnMoveToNextArea;
		GameEvents.only.punchHit += OnPunchHit;
	}

	private void OnDisable()
	{
		GameEvents.only.moveToNextArea -= OnMoveToNextArea;
		GameEvents.only.punchHit -= OnPunchHit;
	}

	private void Start ()
	{
		Vector3 rot = transform.eulerAngles;

		_initRotAxisX = rot.x;
		
		_rotY = rot.y;
		_rotX = rot.x;
	}

	private void Update()
	{
		//print(InputExtensions.GetInputPosition().y / Screen.height);
	}

	public void SetReticleStatus(bool isOn)
	{
		reticle.enabled = isOn;
	}

	public void FindTarget()
    {
	    reticle.color = findTargetColor;
	}

    public void LoseTarget()
    {
	    reticle.color = missingTargetColor;
    }

    public void Aim(Vector2 inputDelta)
    {
		_rotY += inputDelta.x * aimSpeedHorizontal * Time.deltaTime;
		_rotX -= inputDelta.y * aimSpeedVertical * Time.deltaTime;
 
		_rotY = Mathf.Clamp(_rotY, -clampAngleHorizontal, clampAngleHorizontal);
		_rotX = Mathf.Clamp(_rotX, _initRotAxisX - clampAngleVertical, _initRotAxisX + clampAngleVertical);

		var newRot = Quaternion.Euler(_rotX, _rotY, 0.0f);
		transform.rotation = newRot;
	}

	private void OnPunchHit()
	{
		_punchHit = transform.DORotate(new Vector3(_initRotAxisX, 0, 0), 1f);
		_rotX = _initRotAxisX;
		_rotY = 0f;
	}
	
	private void OnMoveToNextArea()
	{
		_punchHit.Kill();
		DOTween.Sequence().Append(transform.DORotate(Vector3.zero, 0.5f));
		_rotX = 0f;
		_rotY = 0f;
	}
}
