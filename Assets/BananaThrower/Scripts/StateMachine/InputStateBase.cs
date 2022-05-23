using DG.Tweening;
using Player;
using UnityEngine;

namespace StateMachine
{
	public class InputStateBase
	{
		public bool IsPersistent;
		protected static PlayerRefBank Player;
		protected static Camera Cam;
		protected static Rigidbody rb;

		protected static float maxRayDistance;
		protected InputStateBase() { }
		
		public InputStateBase(PlayerRefBank player)
		{
			Player = player;
			maxRayDistance = BananaThrower.LevelFlowController.only.maxRayDistance;
		}

		public virtual void OnEnter()
		{
			
		}

		public virtual void Execute() { }

		public virtual void FixedExecute() { }

		public virtual void OnExit() { }

		public static void print(object message) => Debug.Log(message);

		private protected static void PopScaleOfSelected(Transform target)
		{
			if (DOTween.IsTweening(target)) return;
			target.DOPunchScale(target.localScale * 0.125f, 0.25f);
		}
		
		public static Vector2 GetInputPosition()
		{
			if (!InputExtensions.GetFingerHeld() && !InputExtensions.GetFingerUp()) return Vector2.zero;

			if (InputExtensions.IsUsingTouch)
				return Input.GetTouch(0).position;
		
			return Input.mousePosition;
		}
	}

	public sealed class DisabledState : InputStateBase
	{
		public override void OnEnter()
		{
			IsPersistent = true;
		}
	}

	public sealed class IdleState : InputStateBase
	{
		public override void OnEnter()
		{
			IsPersistent = true;
		}
	}
}