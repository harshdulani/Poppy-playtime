using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledParticles : MonoBehaviour {

    public RectTransform targetForParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Coin")
        {
            ParticleControl.PlayControlParticles(collision.transform.position, targetForParticles);
        }
    }
}
