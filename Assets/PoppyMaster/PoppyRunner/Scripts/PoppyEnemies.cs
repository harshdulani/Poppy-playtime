using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PoppyEnemies : MonoBehaviour
{
    public static PoppyEnemies instance;

    public List<PoppyEnemy> poppyEnemiesGroup;

    private void Awake()
    {
        instance = this;
    }
   
    void Start()
    {
      //  poppyEnemiesGroup = GetComponentsInChildren<PoppyEnemy>().ToList();
    }

    public void RunBack()
    {
        for (int i = 0; i < poppyEnemiesGroup.Count; i++)
        {
            poppyEnemiesGroup[i].RunBack();
        }
    }

    public bool IsPoppiesCleared()
    {
        for (int i = 0; i < poppyEnemiesGroup.Count; i++)
        {
            if (!poppyEnemiesGroup[i].gotHit)
                return false;
        }

        return true;
    }
   
}
