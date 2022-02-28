using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FireManDeath : MonoBehaviour
{
    public bool off;
    public List<GameObject> bodyIcePieces;
    public Material iceMaterial;
    public GameObject hitEffect;
    public Color mainColor;
    public Color emissionColor;
    public EGA_Laser laser;
    public ParticleSystem crackEffect;

    Animator animator;
    bool switchOn;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Idle", Random.Range(1, 3));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InGamePanel.instance.levelNum.SetActive(false);
            PlayerController.instance.stopController = true;
            PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("Idle");
            Jump();
        }
    }

    void OnJumpComplete()
    {
        CameraFollow.instance.enabled = false;
        CameraShake.instance.enabled = true;
        CameraShake.instance.shakeDuration = 0.15f;
        crackEffect.Play();
        if (GameEssentials.instance)
            GameEssentials.instance.shm.PlayGroundSlamSound();
        PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("Shoot");
        PlayerController.instance.laser.gameObject.SetActive(true);
        StartCoroutine("Death");
    }

    void Jump()
    {
        transform.DOLocalMoveY(-0.25f, 0.2f).SetEase(Ease.OutFlash).OnComplete(()=> {  OnJumpComplete(); });
    }

    private void Update()
    {
        if (switchOn) 
        {
            laser.SetLaser(transform.position + new Vector3(0,1.5f,0));
            GameEssentials.instance.shm.Vibrate(15);
        }
    }

    public IEnumerator Death()
    {
        if (GameEssentials.instance)
            GameEssentials.instance.shm.Invoke("PlayBossIceShootSound", 0.25f);
        yield return new WaitForSeconds(1.5f);
        switchOn = true;
        animator.SetTrigger("Still");
       
        Material thisMaterial = transform.GetChild(1).GetComponent<Renderer>().material;
        thisMaterial.DOColor(mainColor, 2);
        //thisMaterial.color = mainColor;
        DOTween.To(() => thisMaterial.color, value => thisMaterial.color = value, mainColor, 2f);
        DOTween.To(() => thisMaterial.GetColor("_EmissionColor"), value => thisMaterial.SetColor("_EmissionColor", value), emissionColor, 2f);
        /*Color emissionColor = thisMaterial.GetColor("_EmissionColor");
        emissionColor*/
       
        hitEffect.SetActive(true);

        for (int i = 0; i < bodyIcePieces.Count; i++)
        {
            bodyIcePieces[i].SetActive(true);
            bodyIcePieces[i].transform.DOScale(Vector3.zero, 0.2f).From();
            yield return new WaitForSeconds(0.075f);
        }
       
        PlayerController.instance.girl.GetComponent<Animator>().SetTrigger("ShootEnd");
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(1).GetComponent<Renderer>().enabled = false;
       
        for (int i = 0; i < bodyIcePieces.Count; i++)
        {
            bodyIcePieces[i].transform.SetParent (Camera.main.transform);
            bodyIcePieces[i].GetComponent<Rigidbody>().isKinematic = false;
            bodyIcePieces[i].GetComponent<Collider>().isTrigger = false;
        }

        if (GameEssentials.instance)
            GameEssentials.instance.shm.PlayBlastSound();
        switchOn = false;
        hitEffect.SetActive(false);
        PlayerController.instance.laser.gameObject.SetActive(false);
        UIManager.instance.LevelCompleted();
        yield return new WaitForSeconds(6f);
        for (int i = 0; i < bodyIcePieces.Count; i++)
        {
            bodyIcePieces[i].gameObject.SetActive(false);
        }

        
    }
}
