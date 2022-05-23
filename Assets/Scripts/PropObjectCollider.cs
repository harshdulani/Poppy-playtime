using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropObjectCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if (other.collider.CompareTag("Ground"))
		{
			transform.tag = "Untagged";
			print("on ground");
		}

		
	}
}
