using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropIfNoLand : MonoBehaviour
{
    RaycastHit hit;
    public void Update()
    {
        Physics.Raycast(transform.position,
               transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
        if (hit.collider==null)
        {
            GetComponent<Bots>().fall();
            GetComponent<Bots>().enabled = false;
            GetComponent<DropIfNoLand>().enabled = false;
        }
    }
}
