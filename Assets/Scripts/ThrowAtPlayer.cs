using DG.Tweening;
using TMPro;
using UnityEngine;

public class ThrowAtPlayer : MonoBehaviour
{
	[Header("Throwing at player"), SerializeField] private bool shouldThrow;
	[SerializeField] private GameObject throwableObject;
	[SerializeField] private Transform throwSocket;
	[SerializeField] private float throwInterval, throwForce, firstThrowDelay;
	
	private static Transform _playerReference;
	private Animator _anim;
	private EnemyPatroller _patroller;

	[Header("Radial Shooter"), SerializeField] private GuiProgressBarUI progressBarUi;
	[SerializeField] private TextMeshProUGUI exclaim;

	public Sequence attackSeq;

	private float ProgressBarValue
	{
		get => progressBarUi.Value;
		set => progressBarUi.Value = value;
	}

	private static readonly int Throw = Animator.StringToHash("throw");
	private bool _isFirstThrow = true;

	private void OnEnable()
	{
		GameEvents.Only.ReachNextArea += OnReachNextArea;
		GameEvents.Only.EnemyKillPlayer += OnEnemyKillPlayer;
	}

	private void OnDisable()
	{
		GameEvents.Only.ReachNextArea -= OnReachNextArea;
		GameEvents.Only.EnemyKillPlayer -= OnEnemyKillPlayer;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		
		TryGetComponent(out _patroller);
		
		if(!_playerReference)
			_playerReference = GameObject.FindGameObjectWithTag("Player").transform;
		
		OnReachNextArea();
	}

	private void StartThrowing()
	{
		attackSeq = DOTween.Sequence();

		if(progressBarUi.isActiveAndEnabled)
			attackSeq.AppendCallback(ResetProgressValue);
		if(exclaim.isActiveAndEnabled)
			attackSeq.AppendCallback(() => exclaim.enabled = false);

		if (exclaim.isActiveAndEnabled)
			attackSeq.Append(DOTween.To(() => ProgressBarValue, value => ProgressBarValue = value, 1f, throwInterval)
				.OnUpdate(() => exclaim.enabled = ProgressBarValue > 0.8f));
		else
			attackSeq.AppendInterval(throwInterval);

		attackSeq.AppendCallback(() => _anim.SetTrigger(Throw));
		
		if (_isFirstThrow && firstThrowDelay > 0f)
		{
			attackSeq.PrependInterval(firstThrowDelay);
			_isFirstThrow = false;
		}

		attackSeq.AppendCallback(StartThrowing);
	}
	
	private void ResetProgressValue() => ProgressBarValue = 0f;
	
	public void StopThrowing()
	{
		if(!shouldThrow) return;
		
		attackSeq.Kill();
		if(progressBarUi)
			progressBarUi.transform.parent.gameObject.SetActive(false);
	}

	public void ThrowOnAnimation()
	{
		var thrower = Instantiate(throwableObject, throwSocket.position, throwSocket.rotation).GetComponent<Rigidbody>();
		thrower.transform.localScale = throwSocket.lossyScale;

		thrower.tag = "EnemyAttack";
		thrower.useGravity = true;
		thrower.isKinematic = false;
		thrower.AddForce((_playerReference.position + Vector3.up * 3f - throwSocket.position).normalized * throwForce, ForceMode.Impulse);
		thrower.AddTorque(transform.right * throwForce, ForceMode.Impulse);
	}

	private void OnReachNextArea()
	{
		if(!shouldThrow) return;
		if(_patroller.myPatrolArea != LevelFlowController.only.currentArea) return;
		
		StartThrowing();
	}

	private void OnEnemyKillPlayer() => StopThrowing();
}