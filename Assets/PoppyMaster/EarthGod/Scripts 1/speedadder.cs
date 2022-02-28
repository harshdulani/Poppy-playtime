using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedadder : MonoBehaviour
{
    public float givenspeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Bots>().Walkspeed = givenspeed; 
    }
}
