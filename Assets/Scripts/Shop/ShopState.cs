using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ShopState
{
	public Dictionary<WeaponType, ShopItemState> weaponStates;
	public Dictionary<ArmsType, ShopItemState> armStates;
	public int LoaderWeapon { get; set; }
	public int SidebarWeapon { get; set;}
	public int CoinCount { get; set;}
	public bool AllWeaponsUnlocked { get; set; }

	public int CurrentSpeedLevel { get; set; }
	public int CurrentPowerLevel { get; set; }

	public ShopState(Dictionary<WeaponType, ShopItemState> newWeaponState, Dictionary<ArmsType, ShopItemState> newArmStates, int newCoinCount, int sidebarSkin, int loaderSkin)
	{
		weaponStates = newWeaponState;
		armStates = newArmStates;
		CoinCount = newCoinCount;
		SidebarWeapon = sidebarSkin;
		LoaderWeapon = loaderSkin;
		AllWeaponsUnlocked = false;
	}
}

public class ShopStateHelpers
{
	private readonly ShopState _shopState;

	public ShopStateHelpers(ShopState shopState)
	{
		_shopState = shopState;
	}

	public ShopState GetState() => _shopState;
	
	public int GetCurrentWeapon() => (int) _shopState.weaponStates.First(state => state.Value == ShopItemState.Selected).Key;
	public int GetCurrentArmsSkin() => (int) _shopState.armStates.First(state => state.Value == ShopItemState.Selected).Key;

	public void SetNewLoaderWeapon(int index) => _shopState.LoaderWeapon = index;
	public void SetNewSideBarWeapon(int index) => _shopState.SidebarWeapon = index;

	public int GetCurrentSpeedLevel() => _shopState.CurrentSpeedLevel;
	public int GetCurrentPowerLevel() => _shopState.CurrentPowerLevel;
	
	public void SetNewSpeedLevel(int level) => _shopState.CurrentSpeedLevel = level;
	public void SetNewPowerLevel(int level) => _shopState.CurrentPowerLevel = level;

	public bool AreAllWeaponsUnlocked()
	{
		if (_shopState.AllWeaponsUnlocked) return true;

		return _shopState.weaponStates.Count(state => state.Value == ShopItemState.Locked) == 0;
	}

	public void AllWeaponsHaveBeenUnlocked() => _shopState.AllWeaponsUnlocked = true;

	public Dictionary<WeaponType, ShopItemState> GetWeaponStates() => _shopState.weaponStates;
	public Dictionary<ArmsType, ShopItemState> GetSkinStates() => _shopState.armStates;
}