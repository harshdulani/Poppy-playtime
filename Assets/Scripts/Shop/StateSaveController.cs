using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class ShopState
{
	public Dictionary<WeaponType, ShopItemState> weaponStates;
	public int coinCount;
	
	public ShopState(Dictionary<WeaponType, ShopItemState> currentState, int newCoinCount)
	{
		weaponStates = currentState;
		coinCount = newCoinCount;
	}
}

public class StateSaveController : MonoBehaviour
{
	public static StateSaveController only;
	
	private string _savePath = "";

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
		
		_savePath = Application.persistentDataPath + "/shopState.save";
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.D)) DeleteSavedState();
	}

	public void SaveCurrentState(Dictionary<WeaponType, ShopItemState> currentShopState, int newCoinCount)
	{
		var save = new ShopState(currentShopState, newCoinCount);

		var binaryFormatter = new BinaryFormatter();
		using (var fileStream = File.Create(_savePath))
		{
			binaryFormatter.Serialize(fileStream, save);
		}

		Debug.Log("Data Saved");
	}
	
	public void SaveCurrentState(ShopState currentShopState)
	{
		var save = new ShopState(currentShopState.weaponStates, currentShopState.coinCount);

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
			print(_savePath);
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

	[ContextMenu("DeleteSavedState")]
	public void DeleteSavedState()
	{
		if(!File.Exists(_savePath)) return;

		File.Delete(_savePath);
	}
	
	private static ShopState InitialiseEmptyState()
	{
		var blank = new Dictionary<WeaponType, ShopItemState> {{WeaponType.Punch, ShopItemState.Selected}};

		for(var i = 1; i < Enum.GetNames(typeof(WeaponType)).Length; i++)
			blank.Add((WeaponType) i, ShopItemState.Locked);
		
		return new ShopState(blank, 0);
	}
}