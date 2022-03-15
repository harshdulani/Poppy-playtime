using System;
using System.Collections.Generic;

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
	
	public int GetCurrentWeapon() => GetFirstSelected(_shopState.weaponStates);

	public int GetCurrentArmsSkin() => GetFirstSelected(_shopState.armStates);

	public void SetNewLoaderWeapon(int index) => _shopState.LoaderWeapon = index;
	public void SetNewSideBarWeapon(int index) => _shopState.SidebarWeapon = index;

	public int GetCurrentSpeedLevel() => _shopState.CurrentSpeedLevel;
	public int GetCurrentPowerLevel() => _shopState.CurrentPowerLevel;
	
	public void SetNewSpeedLevel(int level) => _shopState.CurrentSpeedLevel = level;
	public void SetNewPowerLevel(int level) => _shopState.CurrentPowerLevel = level;

	private static int GetFirstSelected<T>(Dictionary<T, ShopItemState> states) where T : Enum
	{
		//starting from -1 to account for case 0
		var index = -1;
		foreach (var state in states)
		{
			index++;
			if (state.Value == ShopItemState.Selected) break;
		}
		return index;
	}

	public bool AreAllWeaponsUnlocked()
	{
		if (_shopState.AllWeaponsUnlocked) return true;

		foreach (var state in _shopState.weaponStates)
			if (state.Value == ShopItemState.Locked)
				return false;

		AllWeaponsHaveBeenUnlocked();
		return true;
	}

	public void AllWeaponsHaveBeenUnlocked() => _shopState.AllWeaponsUnlocked = true;

	public Dictionary<WeaponType, ShopItemState> GetWeaponStates() => _shopState.weaponStates;
	public Dictionary<ArmsType, ShopItemState> GetSkinStates() => _shopState.armStates;
}