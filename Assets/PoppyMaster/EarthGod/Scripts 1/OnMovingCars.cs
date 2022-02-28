using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMovingCars : MonoBehaviour
{
    public List<GameObject> cars;
    public void Start()
    {
        //   cars.SetActive(false);
    }
    public void startmoveingcars()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].GetComponent<Carmovement>().walk = true;
        }
    }
}
