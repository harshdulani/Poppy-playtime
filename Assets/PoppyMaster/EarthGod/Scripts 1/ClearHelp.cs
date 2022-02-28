using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearHelp : MonoBehaviour
{

    public GameObject HandImage;
    public GameObject taphere;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
        if(Input.GetMouseButton(0))
        {
            HandImage.SetActive(false);
            taphere.SetActive(false);
            GetComponent<ClearHelp>().enabled = false;
        }
    }
}
