using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnnextgainton : MonoBehaviour
{
    public GameObject nextgaint;
    // Start is called before the first frame update
    void Start()
    {
        nextgaint.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nextgaint.SetActive(true);
        }
    }
}
