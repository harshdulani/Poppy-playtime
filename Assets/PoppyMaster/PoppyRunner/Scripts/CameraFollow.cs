using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;
    public Transform target;
    public Vector3 offset;

    PoppyEnemies poppyEnemies { get { return PoppyEnemies.instance; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        offset = target.position - transform.position;
    }

    void LateUpdate()
    {
        if (!poppyEnemies.IsPoppiesCleared())
            transform.position = Vector3.MoveTowards(transform.position, target.position - offset, 30f * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, target.position - offset, 300f * Time.deltaTime);

    }
}
