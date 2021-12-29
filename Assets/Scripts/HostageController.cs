using UnityEngine;

public class HostageController : MonoBehaviour
{
	public bool isRagdoll;

	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private AudioClip die, nice1, nice2;
	[SerializeField] private GameObject helpCanvas;
	
	private Animator _anim;
	private AudioSource _audioSource;
	private static readonly int NeedsHelp = Animator.StringToHash("needsHelp");
	private static readonly int Saved = Animator.StringToHash("saved");

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
		GameEvents.only.gameEnd += OnGameEnd;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
		GameEvents.only.gameEnd -= OnGameEnd;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
	}

	public void GoRagdoll(Vector3 direction)
	{
		if(isRagdoll) return;
		
		_anim.enabled = false;
		_anim.applyRootMotion = false;
		isRagdoll = true;

		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(direction * 10f, ForceMode.Impulse);
		}

		foreach (var rb in rigidbodies)
			rb.tag = "Untagged";
		
		helpCanvas.SetActive(false);
		_audioSource.PlayOneShot(die);
	}
	
	private void OnTapToPlay()
	{
		_anim.SetTrigger(NeedsHelp);
		helpCanvas.SetActive(true);
	}
	
	private void OnGameEnd()
	{
		if(isRagdoll) return;
		
		_anim.SetTrigger(Saved);
		helpCanvas.SetActive(false);
		_audioSource.PlayOneShot(Random.Range(0f, 1f) > 0.5f ? nice1 : nice2);
	}
}
