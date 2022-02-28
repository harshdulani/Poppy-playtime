using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class cloudrot : MonoBehaviour
{

    public void Update()
    {
        transform.Rotate(transform.up, 1 * Time.smoothDeltaTime);
    }
   
   
}
