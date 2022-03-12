using DG.Tweening;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IWantsAds
{
	[SerializeField] private bool randomiseIndex;
	[SerializeField] private int myWeaponIndex;

	[SerializeField] private Transform weaponParent;

	private void Start()
	{
		//make sure you dont randomise to the actively selected weapon
		if (randomiseIndex)
			myWeaponIndex = Random.Range(0, weaponParent.childCount);
		
		for(var i = 0; i < weaponParent.childCount; i++)
			weaponParent.GetChild(i).gameObject.SetActive(i == myWeaponIndex);
	}

	[ContextMenu("ChangeWeaponOnDisplay")]
	private void Validate()
	{
		for(var i = 0; i < weaponParent.childCount; i++)
			weaponParent.GetChild(i).gameObject.SetActive(i == myWeaponIndex);
	}

	private void DestroyPickup()
	{
		transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack);
	}

	private void AcceptWeapon()
	{
		DestroyPickup();
		InputHandler.Only.GetRightHand().UpdateEquippedWeaponsSkin(false, myWeaponIndex);
	}
	
	private void RejectWeapon()
	{
		DestroyPickup();
	}

	private void PlayerInteractWithPickup()
	{
		if(ApplovinManager.instance)
			ApplovinManager.instance.ShowRewardedAds();
		
		AdsMediator.StartListeningForAds(this);
	}
	
	public void OnAdRewardReceived(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		AcceptWeapon();
		
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		RejectWeapon();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdFailedToLoad(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		RejectWeapon();
		AdsMediator.StopListeningForAds(this);
	}

	public void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		RejectWeapon();
		AdsMediator.StopListeningForAds(this);
	}
}