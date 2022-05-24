using UnityEngine;
using DG.Tweening;

public class VehicleMovement : MonoBehaviour
{
	public static float MovementSpeed;
	
	[SerializeField] private Transform endPoint;
	[SerializeField] private float travelTime;

	[Header("Bomb"), SerializeField] private Transform bombInLevel;
	[SerializeField] private float bombAppearsAtTravelPercent;

	private Tween _shakeYTween, _shakeXTween, _moveTween;
	private Transform _root;
	private float _initZPos;

	private void OnEnable()
	{
		GameEvents.Only.TapToPlay += OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer += OnGameLose;
	}

	private void OnDisable()
	{
		GameEvents.Only.TapToPlay -= OnTapToPlay;
		GameEvents.Only.EnemyKillPlayer -= OnGameLose;
	}
	
	private void Start()
	{
		_root = transform.root;
		_initZPos = _root.position.z;
	}

	private void ShakeVehicle()
	{
		var localPosition = transform.localPosition;
		var yPos = localPosition.y;
		var xPos = localPosition.x;
		
		_shakeYTween = transform.DOLocalMoveY(yPos + 0.01f, 0.2f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
		_shakeXTween = transform.DOLocalMoveX(xPos + 0.01f, 0.2f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
	}

	private void MoveVehicle()
	{
		_moveTween = transform.root.DOMoveZ(endPoint.transform.position.z, travelTime)
			.SetEase(Ease.Linear)
			.OnUpdate(() =>
		{
			if(!VehicleDistanceUiCanvasController.only) return;
			VehicleDistanceUiCanvasController.only.SetDistanceUI(
				Mathf.InverseLerp(
					endPoint.transform.position.z, _initZPos, _root.position.z));
		}).OnComplete(() =>
		{
			if(BananaThrower.LevelFlowController.only.enemiesKilledInCurrentArea == BananaThrower.LevelFlowController.only.enemiesInCurrentArea) 
				GameEvents.Only.InvokeGameEnd();
			else
				GameEvents.Only.InvokeEnemyKillPlayer();
		});

		MovementSpeed = (endPoint.transform.position.z - _initZPos) / travelTime;
		
		if(bombInLevel)
			DOVirtual.DelayedCall(travelTime * bombAppearsAtTravelPercent, () => bombInLevel.DOLocalMoveX(0f, 1f));
	}
	
	private void OnTapToPlay()
	{
		ShakeVehicle();
		MoveVehicle();
	}

	private void OnGameLose()
	{
		_moveTween.Kill();
		_shakeXTween.Kill();
		_shakeYTween.Kill();
	}
}
