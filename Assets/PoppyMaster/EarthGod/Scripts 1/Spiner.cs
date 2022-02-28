using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiner : MonoBehaviour
{
    public Vector3 Direction;
    public GameObject backuppoint;
    public bool ComeBack;
    public bool Spin;
    public void Start()
    {
        Spin = true;
    }
    void Update()
    {
        if (Spin)
        {
            transform.Rotate(Direction * 20 *Time.deltaTime);
        }
        if (ComeBack)
        {
            transform.position = Vector3.MoveTowards(transform.position, backuppoint.transform.position, 125 * Time.deltaTime);
            transform.localScale = Vector3.MoveTowards(transform.localScale, backuppoint.transform.localScale, 125 * Time.deltaTime);
            if (Vector3.Distance(transform.position, backuppoint.transform.position) < 0.15f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, backuppoint.transform.localRotation, 125 * Time.deltaTime);
                Spin = false;
                ComeBack = false;
                backuppoint.SetActive(true);
                backuppoint.transform.GetComponentInParent<Arm>().Hammerisback();
                //if (AudioManager.instance)
                //{
                //    AudioManager.instance.Pause("Hammermove");
                //    AudioManager.instance.Play("Hammerstop");
                //}
                Destroy(gameObject);

            }
        }
    }
}
