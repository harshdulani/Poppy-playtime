using UnityEngine;

public class ShopStateController : MonoBehaviour
{
	public static ShopStateController only;
	
	//cant use call application.datapath from instance field initializer
	public static ShopStateSerializer ShopStateSerializer { get; private set; }
	public static ShopStateHelpers CurrentState { get; private set; } 

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
		
		ShopStateSerializer = new ShopStateSerializer(Application.persistentDataPath + "/shopState.save");
		CurrentState = new ShopStateHelpers(ShopStateSerializer.LoadSavedState());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.D)) ShopStateSerializer.DeleteSavedState();
	}
}