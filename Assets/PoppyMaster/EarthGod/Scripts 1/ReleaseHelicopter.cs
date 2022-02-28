using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReleaseHelicopter : MonoBehaviour
{
    public GameObject helicopter;
    public GameObject helicoptertarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void relese()
    {
        helicopter.transform.parent = null;
        helicopter.transform.DOMove(helicoptertarget.transform.position, 8);
    }
}
