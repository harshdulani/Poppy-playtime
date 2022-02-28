using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cargroponner : MonoBehaviour
{
    public GameObject cars; 
    void Start()
    {
        cars.SetActive(false);  
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cars.SetActive(true);
            GetComponent<SupercrackTrigger>().kill = true;
        }
    }
}
