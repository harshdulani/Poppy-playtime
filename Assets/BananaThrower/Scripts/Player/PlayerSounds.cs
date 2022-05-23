using UnityEngine;

namespace Player
{
	public class PlayerSounds : MonoBehaviour
	{
		public static PlayerSounds only;
		
		public AudioClip footsteps;
	
		private AudioSource _source;

		private void OnEnable()
		{
			
		}

		private void OnDisable()
		{
			
		}

		private void Awake()
		{
			if (!only) only = this;
			else Destroy(gameObject);
		}

		private void Start() => _source = GetComponent<AudioSource>();

		public void PlaySound(AudioClip clip, float volumeScale) => _source.PlayOneShot(clip, volumeScale);

		public void PlayFootSteps()
		{
			_source.clip = footsteps;
			_source.Play();
		}

		private void OnMoveToNextArea()
		{
			PlayFootSteps();
		}

		private void StopFootSteps()
		{
			_source.Stop();
		}
	}
}