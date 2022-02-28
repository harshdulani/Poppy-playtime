using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rock : MonoBehaviour
{
    public Rigidbody rb;
    public BoxCollider myBoxCollider;
    public SphereCollider mySphereCollider;

    public void ActivateRB()
    {
        print("activate");
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 250, ForceMode.Impulse);
        rb.AddTorque(transform.forward * 250, ForceMode.Impulse);
    }

    void IgnoreColliders( List<Rigidbody> cols)
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

            myBoxCollider.enabled = false;
            other.GetComponent<PoppyEnemy>().ThrowSideWays();
            IgnoreColliders(other.GetComponent<PoppyEnemy>().bodyParts);
            if (GameEssentials.instance)
                GameEssentials.instance.shm.Invoke("PlayEnemySound", 0.25f);
        }

        if (other.CompareTag("Player"))
        {
            UIManager.instance.LevelFailed();
            PlayerController.instance.GameOver();
            PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("Fall");
            if (GameEssentials.instance)
                GameEssentials.instance.shm.PlayEnemySound();
        }

        if (other.CompareTag("Hand"))
        {
            ActivateRB();
            if (GameEssentials.instance)
                GameEssentials.instance.shm.PlayPunchSound();
        }
    }

    public void ScaleABit()
    {
        transform.DOScale(Vector3.one * 13, 0.1f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo);
    }
}
