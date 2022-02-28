using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject);
       // speed = 3;
        // GetComponent<BoxCollider>().enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
      //  transform.position = new Vector3(transform.position.x, transform.position.y+ speed * Time.deltaTime, transform.position.z);
    }
}
