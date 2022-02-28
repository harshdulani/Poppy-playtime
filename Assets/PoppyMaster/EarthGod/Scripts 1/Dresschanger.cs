using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class axechanger : MonoBehaviour
{
    public List<GameObject> dress;
    [Range(0, 160)]
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
