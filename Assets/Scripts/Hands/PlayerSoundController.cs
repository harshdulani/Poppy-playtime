using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
	public static PlayerSoundController only;
	
	public AudioClip[] prePunch, punch;
	public AudioClip ziplineLeave, ziplineGo, ziplineCome;
	public AudioClip footsteps; 
	
	private AudioSource _source;

	private void OnEnable()
	{
		GameEvents.only.reachNextArea += StopFootSteps;
	}

	private void OnDisable()
	{
		GameEvents.only.reachNextArea -= StopFootSteps;
	}

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		_source = GetComponent<AudioSource>();
	}

	public void PlaySound(AudioClip clip, float volumeScale) => _source.PlayOneShot(clip, volumeScale);

	public void PlayFootSteps()
	{
		_source.clip = footsteps;
		_source.Play();
	}

	private void StopFootSteps()
	{
		_source.Stop();
	}

	public void ZiplineGo()
	{
		_source.PlayOneShot(ziplineGo);
	}

	public void ZiplineCome()
	{
		_source.PlayOneShot(ziplineCome);
	}
}