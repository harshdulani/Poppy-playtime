using DG.Tweening;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IWantsAds
{
	[SerializeField] private bool randomiseIndex;
	[ContextMenuItem("Reflect Changes To GameObject", nameof(Validate)), SerializeField] private WeaponType myWeaponType;

	[SerializeField] private Transform weaponParent;

	[Header("Sine Wave Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude;
	private Vector3 _previousSine, _previousSineRot;
	private int _myWeaponIndex;

	private void Start()
	{
		_myWeaponIndex = (int) myWeaponType;
		
		//make sure you dont randomise to the actively selected weapon
		if (randomiseIndex)
		{
			while(_myWeaponIndex == ShopStateController.CurrentState.GetCurrentWeapon())
				_myWeaponIndex = Random.Range(0, weaponParent.childCount);
		}
		
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
		transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
		GameEvents.only.InvokePropDestroy(transform);
	}

	public void PlayerInteractWithPickup()
	{
		if(!ApplovinManager.instance)
		{
			DestroyPickup();
			return;
		}
		
		StartWaiting();
		TimeController.only.SlowDownTime(0f);
		AdsMediator.StartListeningForAds(this);
		ApplovinManager.instance.ShowRewardedAds();
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

	private void StopWaiting()
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