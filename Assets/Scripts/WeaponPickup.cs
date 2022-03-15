using DG.Tweening;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IWantsAds
{
	[ContextMenuItem("Reflect Changes To GameObject", nameof(Validate)), SerializeField] private WeaponType myWeaponType;

	[SerializeField] private Transform weaponParent;

	[Header("Sine Wave Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;

	private Collider _collider;
	private Vector3 _previousSine, _previousSineRot;
	private int _myWeaponIndex;
	private bool _isKilled;

	private void OnEnable()
	{
		GameEvents.only.tapToPlay += OnTapToPlay;
	}

	private void OnDisable()
	{
		GameEvents.only.tapToPlay -= OnTapToPlay;
	}

	private void Start()
	{
		_collider = GetComponent<Collider>();
		_myWeaponIndex = (int) myWeaponType;

		for(var i = 0; i < weaponParent.childCount; i++)
			weaponParent.GetChild(i).gameObject.SetActive(i == _myWeaponIndex);
	}

	private void Update() => SineWave();

	private void SineWave()
	{
		var sineY = Mathf.Sin(Time.time);
		
		var sine = Vector3.up * sineY;
		var sineRot = Vector3.up * sineY;
		
		transform.position += (sine - _previousSine) * magnitude;
		weaponParent.rotation *= Quaternion.Euler((sineRot - _previousSineRot) * rotationMagnitude);
		
		_previousSine = sine;
		_previousSineRot = sineRot;
	}

	private void Validate()
	{
		_myWeaponIndex = (int) myWeaponType;
		for(var i = 0; i < weaponParent.childCount; i++)
			weaponParent.GetChild(i).gameObject.SetActive(i == _myWeaponIndex);
	}

	private void DestroyPickup()
	{
		_isKilled = true;
		
		GameEvents.only.InvokePropDestroy(transform);
		transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
	}

	public void PlayerInteractWithPickup()
	{
		if(_isKilled) return;
		_isKilled = true;
		_collider.enabled = false;
		
		if(!ApplovinManager.instance)
		{
			DestroyPickup();
			return;
		}
		
		AdsMediator.StartListeningForAds(this);
		StartWaiting();
		TimeController.only.SlowDownTime(0f);
		ApplovinManager.instance.ShowRewardedAds();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_isKilled) return;
		if (!other.collider.TryGetComponent(out RagdollLimbController raghu)) return;
		
		DestroyPickup();
	}

	private void OnTapToPlay()
	{
		for(var i = 0; i < weaponParent.childCount; i++)
			weaponParent.GetChild(i).gameObject.SetActive(i == _myWeaponIndex);
	}

	private void StartWaiting()
	{
		InputHandler.Only.userIsWatchingAnAdForPickup = true;
	}

	private void EndAds()
	{
		DestroyPickup();
		
		StopWaiting();
		TimeController.only.RevertTime();	
	}

	private static void StopWaiting()
	{
		InputHandler.Only.userIsWatchingAnAdForPickup = false;
	}
	
	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		weaponParent.gameObject.SetActive(false);
		InputHandler.Only.GetRightHand().UpdateEquippedWeaponsSkin(false, _myWeaponIndex);
		
		EndAds();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		EndAds();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		EndAds();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		EndAds();
		AdsMediator.StopListeningForAds(this);
	}
}