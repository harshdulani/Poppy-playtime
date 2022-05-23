using UnityEngine;

namespace Player
{
	public class HandAnimationHelper : MonoBehaviour
	{
		private PlayerRefBank _my;

		private void Start() => _my = GetComponentInParent<PlayerRefBank>();

		public void ThrowOnAnimation()
		{
			_my.Thrower.ThrowOnAnimation();
			AudioManager.instance.Play("swoosh" + Random.Range(1, 3));
		}
	}
}