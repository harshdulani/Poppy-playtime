using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupercrackTrigger : MonoBehaviour
{
    public GameObject cracker;
    public bool kill;
    void Start()
    {
        cracker.SetActive(false);
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&kill)
        {
            cracker.SetActive(true);
            kill = true;
            cracker.GetComponent<SuperCracker>().StartKilling();
            SendWave.instance.LeftArmAnimator.SetTrigger("HammerHit");
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (SendWave.instance)
            {
                SendWave.instance.notover = false;
            }
            kill = true;
           
        }
    }
}
