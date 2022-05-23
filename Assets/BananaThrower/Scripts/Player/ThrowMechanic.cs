using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class ThrowMechanic : MonoBehaviour
	{
		[SerializeField] private GameObject bananaPrefab;
		[SerializeField] private Transform startPoint;
		[SerializeField] private float trajectoryMaxHeight = 1f, bananaTorque;
	
		[SerializeField] private bool showDebugPath;
		private readonly List<Transform> _drawTrajectory = new List<Transform>();

		private PlayerRefBank _my;
	
		private Transform _hitTransform;
		private Vector3 _lastHitOffset;
		private const float Gravity = -9.81f;

		private static readonly int Aim = Animator.StringToHash("aim");

		private void Start()
		{
			_my = GetComponent<PlayerRefBank>();
		}

		public void NewShot(Transform hitTransform, Vector3 hitPoint)
		{
			_hitTransform = hitTransform;
			
			_lastHitOffset = hitPoint - hitTransform.position;

			_my.Anim.SetTrigger(Aim);
		}

		public void ThrowOnAnimation()
		{
			var idealDest = _hitTransform.position + _lastHitOffset;
			if(!BananaThrower.LevelFlowController.only.isBeingChased) 
				LaunchBanana(idealDest);
			else
			{
				//unreliable, take truck forward
				var travelPerSecond = Vector3.forward * (VehicleMovement.MovementSpeed);
				var idealTrajectorytime = 
					CalculateInitialVelocity(startPoint.position, idealDest, out _);

				var physicalPosition = idealDest + travelPerSecond * idealTrajectorytime;
				LaunchBanana(physicalPosition);
			}

			var initScale = startPoint.localScale;
		
			startPoint.localScale = Vector3.zero;
			DOVirtual.DelayedCall(0.15f, () => startPoint.DOScale(initScale, 0.35f).SetEase(Ease.OutBack));
		}

		private void LaunchBanana(Vector3 hitPoint)
		{
			var kela = Instantiate(bananaPrefab, startPoint.position, startPoint.rotation);

			var rb = kela.GetComponent<Rigidbody>();
			if(showDebugPath)
				DrawPath(startPoint.position, hitPoint);

			CalculateInitialVelocity(startPoint.position, hitPoint, out var initialVelocity);
			rb.AddForce(initialVelocity, ForceMode.VelocityChange);
			rb.AddTorque(Vector3.right * bananaTorque, ForceMode.VelocityChange);
		}

		private float CalculateInitialVelocity(Vector3 startPosition, Vector3 endPoint, out Vector3 initialVelocity)
		{
			//Jai Sebastian Maharaj ki
			var displacementY = endPoint.y - startPosition.y;
			var displacementXZ = new Vector3(endPoint.x - startPosition.x, 0, endPoint.z - startPosition.z);

			var trajectoryHeight = displacementY > trajectoryMaxHeight
				? displacementY + .2f
				: trajectoryMaxHeight;

			var time = Mathf.Sqrt(-2 * trajectoryHeight / Gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / Gravity);
			var velocityY = Vector3.up * Mathf.Sqrt(-2 * Gravity * trajectoryHeight);
			var velocityXZ = displacementXZ / time;

			initialVelocity = velocityXZ + velocityY * -Mathf.Sign(Gravity);

			return time;
		}

		private void DrawPath(Vector3 startPosition, Vector3 endPoint)
		{
			var time = CalculateInitialVelocity(startPosition, endPoint, out var initialVelocity);
			var previousDrawPoint = startPosition;

			const int resolution = 30;
			for (var i = 1; i <= resolution; i++)
			{
				var simulationTime = i / (float) resolution * time;
				var displacement = initialVelocity * simulationTime + Vector3.up * (Gravity * simulationTime * simulationTime) / 2f;
				var drawPoint = startPosition + displacement;
				if (i < resolution && _drawTrajectory.Count > 0)
					_drawTrajectory[i - 1].transform.position = drawPoint;
				Debug.DrawLine(previousDrawPoint, drawPoint, Color.Lerp(Color.red, Color.yellow, i / (float) resolution), 2f);
				previousDrawPoint = drawPoint;
			}
		}
	}
}