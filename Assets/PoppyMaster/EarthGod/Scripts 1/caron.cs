using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class caron : MonoBehaviour
{
    public GameObject cars;
    public GameObject crack;
    public GameObject secondbrokenplatform;
    public GameObject b1;
    void Start()
    {
        cars.SetActive(false);
        secondbrokenplatform.SetActive(false);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            cars.SetActive(true);
            SendWave.instance.Cracker = crack;
            secondbrokenplatform.SetActive(true);
            b1.SetActive(false);
        }
    }
}
