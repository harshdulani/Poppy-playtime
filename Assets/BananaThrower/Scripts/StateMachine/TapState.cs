using DG.Tweening;
using UnityEngine;

namespace StateMachine
{
	public sealed class TapState : InputStateBase
	{
		private RaycastHit _lastHit;
		private Vector3 _lastHitOffset;

		public override void OnEnter()
		{
			IsPersistent = false;
		}

		public override void Execute()
		{
			if(!InputExtensions.GetFingerUp())
			{
				InputHandler.AssignNewState(InputState.Idle);
				return;
			}
			
			var ray = Player.Camera.ScreenPointToRay(GetInputPosition());

			if (!Physics.Raycast(ray, out var hit, maxRayDistance)) return;

			
			print(hit.transform + " hit");

			if(AudioManager.instance)
				AudioManager.instance.Play("Button");
			
			if(hit.transform.CompareTag("Target"))
				PopScaleOfSelected(hit.transform.root);

			Player.Thrower.NewShot(hit.transform, hit.point);
			InputHandler.AssignNewState(InputState.Idle);
		}
	}
}