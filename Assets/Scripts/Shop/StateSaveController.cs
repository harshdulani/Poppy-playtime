using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class StateSaveController : MonoBehaviour
{
	public static StateSaveController only;
	
	private string _savePath;

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
	}

	void Start () 
	{
		_savePath = Application.persistentDataPath + "/shopState.save";
	}
	
	public void SaveCurrentState(Dictionary<WeaponType, ShopItemState> currentState)
	{
		var save = new ShopState(currentState);

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
	
	private ShopState InitialiseEmptyState()
	{
		var blank = new Dictionary<WeaponType, ShopItemState> {{WeaponType.Punch, ShopItemState.Selected}};

		for(var i = 1; i < Enum.GetNames(typeof(WeaponType)).Length; i++)
			blank.Add((WeaponType) i, ShopItemState.Locked);
		
		return new ShopState(blank);
	}
}

[Serializable]
public class ShopState
{
	public Dictionary<WeaponType, ShopItemState> weaponStates;

	public ShopState(Dictionary<WeaponType, ShopItemState> currentState) => weaponStates = currentState;
}