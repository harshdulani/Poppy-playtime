using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bridge : MonoBehaviour
{
    public List<GameObject> cubes;
    public List<GameObject> barrels;
    public TextMeshPro taphere;
    public SpriteRenderer hand;

    void Start()
    {
        
    }

    public void ActivateBarrels()
    {
        for (int i = 0; i < barrels.Count; i++)
        {
            barrels[i].GetComponent<Rigidbody>().isKinematic = false;
            barrels[i].GetComponent<Rigidbody>().AddForce(transform.forward * Random.Range(50,75), ForceMode.Impulse);
        }
    }

    public void BlastBarrel(GameObject barrel)
    {
        barrel.GetComponent<Rigidbody>().isKinematic = true;

        for (int j = 0; j < barrel.transform.childCount; j++)
        {
            barrel.transform.GetChild(j).GetComponent<Rigidbody>().isKinematic = false;
            barrel.transform.GetChild(j).GetComponent<Rigidbody>().AddExplosionForce(7.5f, barrel.transform.position,0.65f,2.5f,ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            GetComponent<Collider>().enabled = false;
            if (GameEssentials.instance)
                GameEssentials.instance.shm.PlayPunchSound();
            ActivateRB();
        }
    }

    public void ActivateRB()
    {
        if (barrels.Count > 0)
        {
            //BlastBarrel();
            ActivateBarrels();
        }
        else 
        {
            for (int i = 0; i < cubes.Count; i++)
            {
                cubes[i].GetComponent<Rigidbody>().isKinematic = false;
                cubes[i].GetComponent<Rigidbody>().AddForce(transform.forward * 25f, ForceMode.Impulse);
            }
        }

        if (hand)
        {
            hand.gameObject.SetActive(false);
            taphere.gameObject.SetActive(false);
        }
        
    }
}
