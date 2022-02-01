using System;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
	Locked, Unlocked, Unavailable, Selected
}
public class ShopItem : MonoBehaviour
{
	[SerializeField] private Image unlocked, selected, icon;
	[SerializeField] private ShopItemState myState; 
	
	public void SetIconSprite(Sprite image) => icon.sprite = image;

	public void SetState(ShopItemState state)
	{
		myState = state;
		switch (myState)
		{
			case ShopItemState.Locked:
				unlocked.enabled = false;
				selected.enabled = false;
				break;
			case ShopItemState.Unlocked:
				unlocked.enabled = true;
				selected.enabled = false;
				break;
			case ShopItemState.Unavailable:
				unlocked.enabled = false;
				selected.enabled = false;
				break;
			case ShopItemState.Selected:
				unlocked.enabled = false;
				selected.enabled = true;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}