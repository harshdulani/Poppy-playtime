using UnityEngine;

public class LeftHand : MonoBehaviour
{
    public bool inUse;

    public Rigidbody rb;
    public Vector3 startPos;
    public Collider myCollider;
    public Transform pivotPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
        myCollider = GetComponent<Collider>();
    }
}
