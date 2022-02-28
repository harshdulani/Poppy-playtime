using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CharaterShiftingfemale : MonoBehaviour
{
    [Range(1, 40)]
    public int BodyNumber;
    public GameObject Heads;
    [Range(0, 65)]
    public int HeadNumber;
    [Range(65, 68)]
    public int GlassNumber;
    public bool changeing;
    void Update()
    {
        if (changeing)
        {
            Heads = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 Head/HEAD_CONTAINER").gameObject;
            transform.GetChild(BodyNumber).gameObject.SetActive(true);
            Heads.transform.GetChild(HeadNumber).gameObject.SetActive(true);
            if (GlassNumber != 65)
            {
                Heads.transform.GetChild(GlassNumber).gameObject.SetActive(true);

            }
            for (int i = 1; i <= 40; i++)
            {
                if (BodyNumber != i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < 66; i++)
            {
                if (HeadNumber != i)
                {
                    Heads.transform.GetChild(i).gameObject.SetActive(false);

                }
            }
            for (int i = 66; i < 69; i++)
            {
                if (GlassNumber != i)
                {
                    Heads.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

    }
}
