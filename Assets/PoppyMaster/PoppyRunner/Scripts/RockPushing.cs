using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class RockPushing : MonoBehaviour
{
    public bool isPushed;
    public bool stopMoving;
    public Rigidbody rb;
    public BoxCollider myBoxCollider;
    public SphereCollider mySphereCollider;
    public List<GameObject> pushPoppys;
    Rigidbody[] temp;

    private void Awake()
    {
        for (int i = 0; i < pushPoppys.Count; i++)
        {
            temp = pushPoppys[i].GetComponentsInChildren<Rigidbody>();
            RB_State(true, temp);
            IgnoreColliders(temp.ToList());
        }
    }

    public void ActivateRB()
    {
        print("activate");

        stopMoving = true;
        rb.isKinematic = false;
        ActivatePoppyRB();
        rb.AddForce(transform.forward * 500, ForceMode.Impulse);
        rb.AddTorque(transform.right * 500, ForceMode.Impulse);
    }

    private void Update()
    {
        if (stopMoving)
            return;

        transform.position -= transform.forward * 10 * Time.deltaTime;
    }

    void IgnoreColliders(List<Rigidbody> cols)
    {
        for (int i = 0; i < cols.Count; i++)
        {
            Physics.IgnoreCollision(cols[i].GetComponent<Collider>(), mySphereCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Poppy"))
        {
            if (PlayerController.instance.stopController)
                return;

            if (GameEssentials.instance)
                GameEssentials.instance.shm.Invoke("PlayEnemySound", 0.25f);
            other.GetComponent<PoppyEnemy>().ThrowSideWays();
            IgnoreColliders(other.GetComponent<PoppyEnemy>().bodyParts);
        }

        if(other.CompareTag("Player"))
        {
            stopMoving = true;
            if (isPushed)
                return;
           
            for (int i = 0; i < pushPoppys.Count; i++)
            {
                pushPoppys[i].GetComponent<Animator>().enabled = false;
            }
            UIManager.instance.LevelFailed();
            PlayerController.instance.GameOver();
            PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("Fall");

            if (GameEssentials.instance)
                GameEssentials.instance.shm.PlayEnemySound();
        }

        if (other.CompareTag("Hand"))
        {
            GetComponent<BoxCollider>().enabled = false;
            if (GameEssentials.instance)
                GameEssentials.instance.shm.PlayPunchSound();
            ActivateRB();
        }
    }

    public void RB_State(bool state, Rigidbody[] bodyParts)
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].isKinematic = state;
        }
    }

    public void ActivatePoppyRB()
    {
        if (pushPoppys.Count <= 0)
            return;

        for (int i = 0; i < pushPoppys.Count; i++)
        {
            pushPoppys[i].transform.parent = null;
            temp = pushPoppys[i].GetComponentsInChildren<Rigidbody>();
           
            RB_State(false, temp);
            pushPoppys[i].GetComponent<Animator>().enabled = false;
           
            if (pushPoppys[i].transform.position.x > 0)
                pushPoppys[i].GetComponentsInChildren<Rigidbody>()[0].AddForce(-transform.forward + new Vector3(1.5f, 0, 0) * 350, ForceMode.Impulse);
            else
                pushPoppys[i].GetComponentsInChildren<Rigidbody>()[0].AddForce(-transform.forward + new Vector3(-1.5f, 0, 0) * 350, ForceMode.Impulse);
        }
    }

    public void ScaleABit()
    {
        transform.DOScale(Vector3.one * 14, 0.1f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo);
    }
}
