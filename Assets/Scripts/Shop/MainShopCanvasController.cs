using System;
using UnityEngine;

public class MainShopCanvasController : MonoBehaviour
{
	[SerializeField] private Transform itemsHolder;
	[SerializeField] private GameObject shopItemPrefab;

	private void Start()
	{
		StateSaveController.only.DeleteSavedState();
		var savedState = StateSaveController.only.LoadSavedState();

		for (var i = 0; i < Enum.GetNames(typeof(WeaponType)).Length; i++)
		{
			var item = Instantiate(shopItemPrefab, itemsHolder).GetComponent<ShopItem>();
			//print($"{(WeaponType) i} is {savedState.weaponStates[(WeaponType) i]}");

			var itemState = savedState.weaponStates[(WeaponType) i];
			item.SetState(itemState);
			item.SetIconSprite(SkinLoader.only.GetSkinSprite(i, itemState == ShopItemState.Locked));
		}
	}
}