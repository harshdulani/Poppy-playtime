using UnityEngine;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    public GameObject lefthand, rightHand;
    public ParticleSystem shotEffect, cakeSmashEffect, hitEffect;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        lefthand = PlayerController.instance.leftHand;
        rightHand = PlayerController.instance.rightHand;
    }


    public void HandMovement(Hand hand, Vector3 pos)
    {
        if (hand.inUse)
            return;

        hand.inUse = true;

        hand.transform.DOScale(Vector3.one * 3f, 0.15f);

        hand.transform.DOMove(pos + new Vector3(0,6f,-3f), 0.35f).SetEase(Ease.Flash)
            .OnComplete(()=> 
            {
                hand.transform.DOLocalRotate(RotationBasedOnWeapon(), 0.125f).SetEase(Ease.Flash).SetLoops(2,LoopType.Yoyo).OnComplete(()=> 
                {
                    if (ItemManager.instance.currentItem == ItemsType.Revolver)
                    {
                        shotEffect.transform.position = pos + new Vector3(0, 10f, -3f);
                        shotEffect.Play();
                    }
                    else if (ItemManager.instance.currentItem == ItemsType.Cake)
                    {
                        cakeSmashEffect.transform.position = pos + new Vector3(0, 10f, -3f);
                        cakeSmashEffect.Play();
                    }
                    else
                    {
                        hitEffect.transform.position = new Vector3(0, 10, -3f) + pos;
                        hitEffect.Play();
                    }

                    if (GameEssentials.instance)
                        GameEssentials.instance.shm.PlayPunchSound();

                    hand.transform.DOScale(Vector3.one, 0.2f);
                    hand.transform.DOLocalMove(hand.startPos, 0.3f).SetEase(Ease.Flash).OnComplete(() => 
                    {
                        if (PlayerController.instance.alternative == 0)
                            PlayerController.instance.alternative = 1;
                        else
                            PlayerController.instance.alternative = 0;

                        hand.inUse = false;
                    });
                });

            });
    }

    public void HandMovementForFallenBridge(Hand hand, Vector3 pos)
    {
        if (hand.inUse)
            return;

        hand.inUse = true;

        hand.transform.DOScale(Vector3.one * 3f, 0.15f);

        hand.transform.DOMove(pos , 0.35f).SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                hand.transform.DOLocalRotate(RotationBasedOnWeapon(), 0.125f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    if (ItemManager.instance.currentItem == ItemsType.Revolver)
                    {
                        shotEffect.transform.position = pos + new Vector3(0, 10f, -3f);
                        shotEffect.Play();
                    }
                    else if (ItemManager.instance.currentItem == ItemsType.Cake)
                    {
                        cakeSmashEffect.transform.position = pos + new Vector3(0, 10f, -3f);
                        cakeSmashEffect.Play();
                    }
                    else
                    {
                        hitEffect.transform.position = new Vector3(0, 10, -3f) + pos;
                        hitEffect.Play();
                    }

                    if (GameEssentials.instance)
                        GameEssentials.instance.shm.PlayPunchSound();

                    hand.transform.DOScale(Vector3.one, 0.2f);
                    hand.transform.DOLocalMove(hand.startPos, 0.3f).SetEase(Ease.Flash).OnComplete(() =>
                    {
                        if (PlayerController.instance.alternative == 0)
                            PlayerController.instance.alternative = 1;
                        else
                            PlayerController.instance.alternative = 0;

                        hand.inUse = false;
                    });
                });

            });
    }

    Vector3 RotationBasedOnWeapon()
    {
        Vector3 temp = Vector3.zero;
        switch (ItemManager.instance.currentItem)
        {
            case ItemsType.Cake:
                temp = new Vector3(-18f, -170f, -72f);
                break;
            case ItemsType.Hammer:
                temp = new Vector3(0, -97f, -200);
                break;
            case ItemsType.Sandle:
                temp = new Vector3(0, -97f, -200);
                break;
            case ItemsType.Shoes:
                temp = new Vector3(0, -97f, -200);
                break;
            case ItemsType.Revolver:
                temp = new Vector3(0, -97f, -120);
                break;
        }

        return temp;
    }
}
