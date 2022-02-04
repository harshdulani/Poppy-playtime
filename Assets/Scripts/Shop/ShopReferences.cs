using UnityEngine;

public class ShopReferences : MonoBehaviour
{
	public static ShopReferences refs;
	
	public MainShopController mainShop;
	public CoinShopController sidebar;
	public SkinLoader skinLoader;

	private void Awake()
	{
		if (!refs) refs = this;
		else Destroy(gameObject);
	}
}