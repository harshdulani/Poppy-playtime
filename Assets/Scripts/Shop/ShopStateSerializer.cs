using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ShopStateSerializer
{
	private readonly string _savePath;

	public ShopStateSerializer(string savePath)
	{
		_savePath = savePath;
	}

	private static ShopState InitialiseEmptyState()
	{
		var blankWeapon = new Dictionary<WeaponType, ShopItemState> {{WeaponType.Punch, ShopItemState.Selected}};

		for(var i = 1; i < MainShopController.GetWeaponSkinCount(); i++)
			blankWeapon.Add((WeaponType) i, ShopItemState.Locked);
		
		var blankArms = new Dictionary<ArmsType, ShopItemState> {{ArmsType.Poppy, ShopItemState.Selected}};

		for(var i = 1; i < MainShopController.GetArmsSkinCount(); i++)
			blankArms.Add((ArmsType) i, ShopItemState.Locked);
		
		return new ShopState(blankWeapon, blankArms, 0, 1, 1);
	}

	public void SaveCurrentState()
	{
		var currentShopState = ShopStateController.CurrentState.GetState();
		var save = new ShopState(currentShopState.weaponStates, currentShopState.armStates, currentShopState.CoinCount, currentShopState.SidebarWeapon, currentShopState.LoaderWeapon);

		var binaryFormatter = new BinaryFormatter();
		using (var fileStream = File.Create(_savePath))
		{
			binaryFormatter.Serialize(fileStream, save);
		}

		Debug.Log("Data Saved");
	}

	public ShopState LoadSavedState()
	{
		if (!File.Exists(_savePath))
		{
			MonoBehaviour.print(_savePath);
			Debug.LogWarning("Save file doesn't exist. Initialising a blank one.");
			return InitialiseEmptyState();
		}
		
		ShopState state;

		var binaryFormatter = new BinaryFormatter();
		using (var fileStream = File.Open(_savePath, FileMode.Open))
			state = (ShopState)binaryFormatter.Deserialize(fileStream);

		Debug.Log("Data Loaded");
		return state;
	}

	//cant call from context menu because path isn't initialised then
	public void DeleteSavedState()
	{
		if (!File.Exists(_savePath))
		{
			MonoBehaviour.print("Data does not Exist at path");
			return;
		}

		MonoBehaviour.print("Data Deleted");
		File.Delete(_savePath);
	}
}