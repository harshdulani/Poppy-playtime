using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MonesterBodyHandler : MonoBehaviour
{
    [Range(0, 7)]
    public int tailsNum;
    [Range(0, 9)]
    public int bodyNum;
    [Range(0, 10)]
    public int eyesNum;
    [Range(0, 9)]
    public int glovesNum;
    [Range(0, 5)]
    public int mainBodyNum;
    [Range(0, 3)]
    public int headpartsNum;
    [Range(0, 14)]
    public int mouthandNosesNum;

    public List<GameObject> mainbody, body, eyes, gloves, headparts, mouthandNoses, tails;


    void Update()
    {
        for (int i = 0; i < mainbody.Count; i++)
        {
            if (i != mainBodyNum)
            {
                mainbody[i].SetActive(false);
            }
            else
            {
                mainbody[i].SetActive(true);
            }
        }
        for (int i = 0; i < body.Count; i++)
        {
            if (i != bodyNum)
            {
                body[i].SetActive(false);
            }
            else
            {
                body[i].SetActive(true);
            }
        }
        for (int i = 0; i < eyes.Count; i++)
        {
            if (i != eyesNum)
            {
                eyes[i].SetActive(false);
            }
            else
            {
                eyes[i].SetActive(true);
            }
        }
        for (int i = 0; i < gloves.Count; i++)
        {
            if (i != glovesNum)
            {
                gloves[i].SetActive(false);
            }
            else
            {
                gloves[i].SetActive(true);
            }
        }
        for (int i = 0; i < headparts.Count; i++)
        {
            if (i != headpartsNum)
            {
                headparts[i].SetActive(false);
            }
            else
            {
                headparts[i].SetActive(true);
            }
        }
        for (int i = 0; i < mouthandNoses.Count; i++)
        {
            if (i != mouthandNosesNum)
            {
                mouthandNoses[i].SetActive(false);
            }
            else
            {
                mouthandNoses[i].SetActive(true);
            }
        }
        for (int i = 0; i < tails.Count; i++)
        {
            if (i != tailsNum)
            {
                tails[i].SetActive(false);
            }
            else
            {
                tails[i].SetActive(true);
            }
        }
    }
}
