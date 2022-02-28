using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLocationPoint : MonoBehaviour
{
    public static RoadLocationPoint instance;
    [Header("Y Value")]
    public float Y;
    [Space(5)]
    [Header("X Values")]
    public float XMin;
    public float XMax;
    [Space(5)]
    [Header("Z Values")]
    public float ZMin;
    public float ZMax;
    private void Awake()
    {
        if(!instance)
        {
           instance=this;
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector3 giveapoint()
    {
       return new Vector3(Random.Range(XMin, XMax), Y, Random.Range(ZMin, ZMax));
    }
}
