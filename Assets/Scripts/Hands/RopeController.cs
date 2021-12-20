using System;
using DG.Tweening;
using UnityEngine;

public class RopeController : MonoBehaviour
{
	[SerializeField] private Transform ropeStart, ropeEnd;
	[SerializeField] private CylinderGeneration cylinder;

	private Vector3 _initPos;
	private Quaternion _initRot;

	private void Start()
	{
		_initPos = ropeEnd.position;
		_initRot = transform.rotation;
	}

	public void UpdateRope()
	{
		var direction = ropeEnd.position - ropeStart.position;
		var magnitude = direction.magnitude;
		cylinder.GetUpdated(direction, magnitude);
	}

	public void ReturnHome()
	{
		transform.DORotateQuaternion(_initRot, 0.2f);
		ropeEnd.DOMove(_initPos, 0.2f)
			.OnUpdate(UpdateRope);
	}
}
