using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenBridge : MonoBehaviour
{
    public static FallenBridge instance;

    public bool isCleared;
    public Rigidbody[] blocks;
    public List<PoppyEnemy> enemiesOnBridge;

   public FallenBridge[] temp;
    void Awake()
    {
        instance = this;

        blocks = GetComponentsInChildren<Rigidbody>();
        temp = FindObjectsOfType<FallenBridge>();
    }

    public void ActivateBlocks(Transform hand)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].isKinematic = false;
            blocks[i].AddExplosionForce(Random.Range(20f, 25f), blocks[i].transform.position, 0.25f, Random.Range(0.5f,0.75f), ForceMode.Impulse);
        }

        for (int i = 0; i < enemiesOnBridge.Count; i++)
        {
            enemiesOnBridge[i].ActivateRB(hand);
        }

        isCleared = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            ActivateBlocks(other.transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Poppy"))
        {
            if (!enemiesOnBridge.Contains(other.GetComponent<PoppyEnemy>()))
            {
                if(other.name.Contains("Gorilla"))
                    enemiesOnBridge.Add(other.GetComponent<PoppyEnemy>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Poppy"))
        {
            if (enemiesOnBridge.Contains(other.GetComponent<PoppyEnemy>()))
            {
                if (other.name.Contains("Gorilla"))
                    enemiesOnBridge.Remove(other.GetComponent<PoppyEnemy>());
            }
        }
    }

    public void ActivateGorillas()
    {
        if (!PoppyEnemies.instance.IsPoppiesCleared())
            return;
       
        for (int i = 0; i < enemiesOnBridge.Count; i++)
        {
            enemiesOnBridge[i].myAnimator.SetTrigger("Run");
            enemiesOnBridge[i].myState = PoppyEnemyState.MoveForward;
        }
    }

    public void CheckForBridgesCleared()
    {
        if (!FallenBrigesCleared())
            return;

        PlayerController.instance.stopMoving = false;
        PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("Walk");
    }


    bool FallenBrigesCleared()
    {

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].isCleared == false)
                return false;
        }

        return true;
    }
}
