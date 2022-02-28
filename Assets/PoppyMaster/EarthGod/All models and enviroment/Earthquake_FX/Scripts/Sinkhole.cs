using UnityEngine;
using System.Collections;

public class Sinkhole : MonoBehaviour
{

    public GameObject sinkholeFX;
    public GameObject sinkholeAnim;
    private Animation ani;

    void Start()
    {

        sinkholeFX.SetActive(false);
        ani = sinkholeAnim.GetComponent<Animation>();

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            StartCoroutine("startSinkhole");

        }


        // Reset earthquake

        if (Input.GetButtonDown("Fire2"))
        {

            ani["Quake"].time = 0.0f;
            ani["Quake"].speed = 0;
            sinkholeAnim.GetComponent<Animation>().Play();
            sinkholeFX.SetActive(false);

        }

    }

    



    IEnumerator startSinkhole()
    {

        ani["Quake"].speed = 1;
        sinkholeFX.SetActive(true);
        sinkholeAnim.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(0.1f);


    }


}
