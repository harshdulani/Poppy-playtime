using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linecrack : MonoBehaviour
{
    public void DropIngCracks(GameObject Crack)
    {
        Crack.AddComponent<TurnOnTriggerForBroken>();
        Crack.AddComponent<Rigidbody>();
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Land"))
        {
            GameObject Collisionwith = collision.gameObject;
            Collisionwith.tag = "Untagged";
            GetComponent<Collider>().isTrigger = false;
            DropIngCracks(Collisionwith);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Land"))
        {
            GameObject Collisionwith = other.gameObject;
            Collisionwith.tag = "Untagged";
            DropIngCracks(Collisionwith);
        }
    }
}
