using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CharaterShifting : MonoBehaviour
{
    [Range(1, 49)]
    public int BodyNumber;
    public GameObject Heads;
    [Range(0, 66)]
    public int HeadNumber;
    [Range(66, 69)]
    public int GlassNumber;
    public bool changeing;
    void Update()
    {
        if (changeing)
        {
            Heads = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 Head/HEAD_CONTAINER").gameObject;
            transform.GetChild(BodyNumber).gameObject.SetActive(true);
           // transform.GetChild(BodyNumber).gameObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
            Heads.transform.GetChild(HeadNumber).gameObject.SetActive(true);
            //Heads.transform.GetChild(HeadNumber).gameObject.GetComponent<MeshRenderer>().updateWhenOffscreen = true;
            if (GlassNumber != 66)
            {
                Heads.transform.GetChild(GlassNumber).gameObject.SetActive(true);
             //   Heads.transform.GetChild(GlassNumber).gameObject.GetComponent<MeshRenderer>().updateWhenOffscreen = true;
            }
            for (int i = 1; i <= 49; i++)
            {
                if (BodyNumber != i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                  
                }
            }
            for (int i = 0; i <= 66; i++)
            {
                if (HeadNumber != i)
                {
                    Heads.transform.GetChild(i).gameObject.SetActive(false);

                }
            }
            for (int i = 67; i <= 69; i++)
            {
                if (GlassNumber != i)
                {
                    Heads.transform.GetChild(i).gameObject.SetActive(false);

                }
            }
        }

    }
}
