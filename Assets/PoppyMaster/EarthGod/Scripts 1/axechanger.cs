using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Dresschanger : MonoBehaviour
{
    public List<GameObject> dress;
    [Range(0, 63)]
    public int thisnumber;
    void Update()
    {
        for (int i = 0; i < dress.Count; i++)
        {
            if (i != thisnumber)
            {
                dress[i].SetActive(false);
            }
            else
            {
                dress[i].SetActive(true);
            }
        }
    }
}
