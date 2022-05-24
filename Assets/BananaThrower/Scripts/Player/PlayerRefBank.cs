using UnityEngine;

namespace Player
{
	public class PlayerRefBank : MonoBehaviour
	{
		public Animator Anim { get; private set; }
		public Camera Camera { get; private set; }
		public PlayerSounds Sounds { get; private set; }

		public ThrowMechanic Thrower { get; private set; }
		
		public Transform bombHolder;

		private void Awake()
		{
			//GameObject.FindGameObjectWithTag("GameController").GetComponent<StateMachine.InputHandler>().SetIkTarget(rightHandTarget);
		}

		private void Start()
		{
			Camera = Camera.main;
			Anim = transform.GetChild(0).GetComponent<Animator>();
			Thrower = GetComponent<ThrowMechanic>();
			Sounds = GetComponent<PlayerSounds>();
		}
	}
}