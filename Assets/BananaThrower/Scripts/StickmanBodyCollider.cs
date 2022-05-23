using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanBodyCollider : MonoBehaviour
{
	//private RagdollController raghu;
	private StickmanRagdollerController raghu;
	private StickmanMovementController stickmanMovement;

	private void Start()
	{
		raghu = transform.root.GetComponent<StickmanRagdollerController>();
		stickmanMovement = transform.root.GetComponent<StickmanMovementController>();
	}
	

	private void OnCollisionEnter(Collision other)
	{
		if(other.transform.root == transform.root) return;

		if (other.transform.CompareTag("PropObject"))
		{
			raghu.GoRagdoll(Vector3.back);
			stickmanMovement.GetHit();
			
		}

		
	}
}
