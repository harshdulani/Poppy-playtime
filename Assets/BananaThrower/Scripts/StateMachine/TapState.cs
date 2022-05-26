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
			Debug.Log(hit.transform);
			//Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);
			if (hit.transform.CompareTag("TrapButton"))
			{
				//reusing this tag for bomb
				if(!hit.transform.TryGetComponent(out BombControl bomb) && !bomb.isHeld)
				{
					InputHandler.AssignNewState(InputState.Idle);
					return;
				}
				
				bomb.HoldBomb(Player.bombHolder);
				Player.Thrower.ThrowBombNextTime();
				return;
			}
			
			if (hit.transform.CompareTag("BlockRaycast"))
			{
				InputHandler.AssignNewState(InputState.Idle);
				return;
			}
			
			if(AudioManager.instance)
				AudioManager.instance.Play("Button");

			if (hit.transform.CompareTag("Target")) PopScaleOfSelected(hit.transform.root);

			Player.Thrower.NewShot(hit.transform, hit.point);
			InputHandler.AssignNewState(InputState.Idle);
		}
	}
}