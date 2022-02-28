using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownPoleAndRun : MonoBehaviour
{
    public Animator animator;
    public GameObject Target1, Target2;
    public bool firstpoint, secondpoint;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Hanging");
        firstpoint = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstpoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target1.transform.position, 25 * Time.deltaTime);
            if (Vector3.Distance(transform.position, Target1.transform.position) < 0.09f)
            {
                firstpoint = false;
                secondpoint = true;
                animator.SetTrigger("Running");
            }
        }
        if (secondpoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, Target2.transform.position, 15 * Time.deltaTime);
            transform.LookAt(Target2.transform);
            if (Vector3.Distance(transform.position, Target2.transform.position) < 0.09f)
            {
                secondpoint = false;
                Destroy(gameObject);
            }
        }
    }

}
