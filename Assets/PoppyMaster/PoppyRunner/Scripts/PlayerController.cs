using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public bool stopController, stopMoving, girlCaptured;
    public float distanceToHit;
    public LayerMask layerMask;

    public GameObject leftHand, rightHand, girl, wings;
    public LineRenderer leftHandLR, rightHandLR, laser;
    
    RaycastHit hit;
    Ray screenPoint;
    PoppyEnemy poppyEnemy;
   
    Camera rayCamera { get { return Camera.main; } }
    Transform myTransfrom { get { return transform; } }
    LeftHand leftHandScript { get { return leftHand.GetComponent<LeftHand>(); } }
    RightHand rightHandScript { get { return rightHand.GetComponent<RightHand>(); } }

    Hand lfHandScript { get { return leftHand.GetComponent<Hand>(); } }
    Hand rtHandScript { get { return rightHand.GetComponent<Hand>(); } }

    PoppyEnemies poppyEnemies { get { return PoppyEnemies.instance; } }

    Transform llrEndPoint { get { return lfHandScript.pivotPoint; } }
    Transform llrStartPoint { get { return leftHandLR.transform; } }

    Transform rlrEndPoint { get { return rtHandScript.pivotPoint; } }
    Transform rlrStartPoint { get { return rightHandLR.transform;  } }


    [HideInInspector]
    public int alternative = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        leftHandLR.positionCount = 2;
        rightHandLR.positionCount = 2;

        if (!stopMoving)
            girl.GetComponent<Animator>().SetTrigger("Walk");

       // AnimateHands();
    }

    void Update()
    {
        if (stopController)
            return;

        if (Input.GetMouseButtonDown(0))
            DetectingPoppy();

        if(!stopMoving)
            Movement();

        if(rtHandScript.inUse)
            rightHandLR.positionCount = 2;
        else
            rightHandLR.positionCount = 0;

        if (lfHandScript.inUse)
            leftHandLR.positionCount = 2;
        else
            leftHandLR.positionCount = 0;


        //if (rightHandScript.inUse)
        //    rightHandLR.positionCount = 2;
        //else
        //    rightHandLR.positionCount = 0;

        //if (leftHandScript.inUse)
        //    leftHandLR.positionCount = 2;
        //else
        //    leftHandLR.positionCount = 0; 



        LeftHandLineRendererPoints();

        RightHandLineRendererPoints();
    }

    void Movement()
    {
        if (!poppyEnemies.IsPoppiesCleared())
            myTransfrom.position += myTransfrom.forward * 20 * Time.deltaTime;
        else
            myTransfrom.position += myTransfrom.forward * 35 * Time.deltaTime;
    }

    void LeftHandLineRendererPoints()
    {
        if (leftHandLR.positionCount == 0)
            return;

        leftHandLR.SetPosition(0, llrStartPoint.position);
        leftHandLR.SetPosition(1, llrEndPoint.position);
    }

    void RightHandLineRendererPoints()
    {
        if (rightHandLR.positionCount == 0)
            return;

        rightHandLR.SetPosition(0, rlrStartPoint.position);
        rightHandLR.SetPosition(1, rlrEndPoint.position);
    }

    void DetectingPoppy()
    {
        if (rtHandScript.inUse && lfHandScript.inUse)
            return;


        screenPoint = rayCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,rayCamera.transform.position.z));

        if (Physics.Raycast(screenPoint, out hit, distanceToHit, layerMask))
        {
            if (hit.collider.CompareTag("Poppy"))
            {
                poppyEnemy = hit.collider.GetComponent<PoppyEnemy>();

                if (poppyEnemy.myState == PoppyEnemyState.ClimbToRoof)
                    return;

                poppyEnemy.ScaleABit();

                poppyEnemy.gotHit = true;

                DeactivateHelp();

                //if(alternative == 0)
                //    RightHandMovement(rightHandScript, poppyEnemy.transform.position);
                //else
                //    LeftHandMovement(leftHandScript, poppyEnemy.transform.position); 
                if (alternative == 0)
                {
                    HandManager.instance.HandMovement(leftHand.GetComponent<Hand>(), poppyEnemy.transform.position);
                }
                else 
                {
                    HandManager.instance.HandMovement(rightHand.GetComponent<Hand>(), poppyEnemy.transform.position);
                }


                //if (rightHandScript.inUse)
                //    LeftHandMovement(leftHandScript, poppyEnemy.transform.position);
                //else if (leftHandScript.inUse)
                //    RightHandMovement(rightHandScript, poppyEnemy.transform.position);
                //else
                //    RightHandMovement(rightHandScript, poppyEnemy.transform.position);
            }

            if (hit.collider.CompareTag("Rock"))
            {
                Rock rock = hit.collider.GetComponent<Rock>();
                rock.ScaleABit();

                if (alternative == 0)
                {
                    HandManager.instance.HandMovementForFallenBridge(leftHand.GetComponent<Hand>(), rock.transform.position + new Vector3(0,-5f,0));
                }
                else
                {
                    HandManager.instance.HandMovementForFallenBridge(rightHand.GetComponent<Hand>(), rock.transform.position + new Vector3(0, -5f, 0));
                }

                //if (rightHandScript.inUse)
                //    LeftHandMovementToRock(leftHandScript, rock, rock.transform.position);
                //else if (leftHandScript.inUse)
                //    RightHandMovementToRock(rightHandScript, rock, rock.transform.position);
                //else
                //    RightHandMovementToRock(rightHandScript, rock, rock.transform.position);
            }

            if (hit.collider.CompareTag("Bridge"))
            {
                Bridge bridge = hit.collider.GetComponent<Bridge>();

                if (alternative == 0)
                {
                    HandManager.instance.HandMovementForFallenBridge(leftHand.GetComponent<Hand>(), bridge.transform.position + new Vector3(0, 15f, 80f));
                }
                else
                {
                    HandManager.instance.HandMovementForFallenBridge(rightHand.GetComponent<Hand>(), bridge.transform.position + new Vector3(0, 15f, 80f));
                }

                //if (rightHandScript.inUse)
                //    LeftHandMovementToBridge(leftHandScript, bridge, bridge.transform.position + new Vector3(0,15f,80f));
                //else if (leftHandScript.inUse)
                //    RightHandMovementToBridge(rightHandScript, bridge, bridge.transform.position + new Vector3(0, 15f, 80f));
                //else
                //    RightHandMovementToBridge(rightHandScript, bridge, bridge.transform.position + new Vector3(0, 15f, 80f));
            }

            if (hit.collider.CompareTag("RockPush"))
            {
                RockPushing rockPush = hit.collider.GetComponent<RockPushing>();
                rockPush.isPushed = true;
                rockPush.ScaleABit();

                if (alternative == 0)
                {
                    HandManager.instance.HandMovementForFallenBridge(leftHand.GetComponent<Hand>(), rockPush.transform.position + new Vector3(0, 0f, -2f));
                }
                else
                {
                    HandManager.instance.HandMovementForFallenBridge(rightHand.GetComponent<Hand>(), rockPush.transform.position + new Vector3(0, 0f, -2f));
                }

                //if (rightHandScript.inUse)
                //    LeftHandMovementToRockPush(leftHandScript, rockPush, rockPush.transform.position + new Vector3(0, 0f, -2f));
                //else if (leftHandScript.inUse)
                //    RightHandMovementToRockPush(rightHandScript, rockPush, rockPush.transform.position + new Vector3(0, 0f, -2f));
                //else
                //    RightHandMovementToRockPush(rightHandScript, rockPush, rockPush.transform.position + new Vector3(0, 0f, -2f));
            }

            if (hit.collider.CompareTag("FallenBridge"))
            {
                if (alternative == 0)
                {
                    HandManager.instance.HandMovementForFallenBridge(leftHand.GetComponent<Hand>(), hit.collider.ClosestPoint(leftHand.transform.position));
                }
                else
                {
                    HandManager.instance.HandMovementForFallenBridge(rightHand.GetComponent<Hand>(),hit.collider.ClosestPoint(rightHand.transform.position));
                }
            }
        }
    }

    void DeactivateHelp()
    {
        if (!Help.instance)
            return;

        Help.instance.DeactiveHelp();
    }
   

    void LeftHandMovement(LeftHand hand, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;

        hand.inUse = true;
        leftHandLR.positionCount = 2;
        if (leftHand.TryGetComponent(out MeshRenderer mr))
            mr.enabled = true;
        hand.transform.DOScale(Vector3.one * 3f, 0.1f).SetEase(Ease.Flash);
        hand.transform.DOMove(pos + new Vector3(0, 12f, 0), 0.5f).SetEase(Ease.Flash).
            OnComplete(() => {
                alternative = 0;
                if (GameEssentials.instance)
                    GameEssentials.instance.shm.PlayPunchSound();
                hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
            OnComplete(() => { 
                hand.inUse = false;
                leftHand.GetComponent<MeshRenderer>().enabled = false;
                
            }); hand.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Flash);
            });
    }

    void RightHandMovement(RightHand hand, Vector3 pos)
    {
        if (hand.inUse)
            return;

        hand.inUse = true;
        rightHandLR.positionCount = 2;
        if(rightHand.TryGetComponent(out MeshRenderer mr))
            mr.enabled = true;
        hand.transform.DOScale(Vector3.one * 3f, 0.1f).SetEase(Ease.Flash);
        hand.transform.DOMove(pos + new Vector3(0, 12f, 0), 0.5f).SetEase(Ease.Flash).
            OnComplete(() => {
                alternative = 1;
                if (GameEssentials.instance)
                    GameEssentials.instance.shm.PlayPunchSound();
                hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
            OnComplete(() => { 
                hand.inUse = false;
                rightHand.GetComponent<MeshRenderer>().enabled = false;
                
            }); hand.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Flash);
            });
    }

    void LeftHandMovementToRock(LeftHand hand, Rock rock, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        leftHandLR.positionCount = 2;
        leftHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, 0), 0.5f).SetEase(Ease.Flash).
           OnComplete(() => {
               if (GameEssentials.instance)
                   GameEssentials.instance.shm.PlayPunchSound();
               rock.ActivateRB();
               hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
           OnComplete(() => {
               hand.inUse = false;
               leftHand.GetComponent<MeshRenderer>().enabled = false;
           });
           });
    }

    void RightHandMovementToRock(RightHand hand, Rock rock, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        rightHandLR.positionCount = 2;
        rightHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, -1), 0.5f).SetEase(Ease.Flash).
            OnComplete(() => {
                if (GameEssentials.instance)
                    GameEssentials.instance.shm.PlayPunchSound();
                rock.ActivateRB();
                hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
            OnComplete(() => {
                hand.inUse = false;
                rightHand.GetComponent<MeshRenderer>().enabled = false;
            });
            });
    }

    void LeftHandMovementToBridge(LeftHand hand, Bridge bridge, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        leftHandLR.positionCount = 2;
        leftHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, 0), 0.5f).SetEase(Ease.Flash).
           OnComplete(() => {
               if (GameEssentials.instance)
                   GameEssentials.instance.shm.PlayPunchSound();
               bridge.ActivateRB();
               hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
           OnComplete(() => {
               hand.inUse = false;
               leftHand.GetComponent<MeshRenderer>().enabled = false;
           });
           });
    }

    void RightHandMovementToBridge(RightHand hand, Bridge bridge, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        rightHandLR.positionCount = 2;
        rightHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, -1), 0.5f).SetEase(Ease.Flash).
            OnComplete(() => {
                if (GameEssentials.instance)
                    GameEssentials.instance.shm.PlayPunchSound();
                bridge.ActivateRB();
                hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
            OnComplete(() => {
                hand.inUse = false;
                rightHand.GetComponent<MeshRenderer>().enabled = false;
            });
            });
    }

    void LeftHandMovementToRockPush(LeftHand hand, RockPushing rockPush, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        leftHandLR.positionCount = 2;
        leftHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, 0), 0.5f).SetEase(Ease.Flash).
           OnComplete(() => {
               if (GameEssentials.instance)
                   GameEssentials.instance.shm.PlayPunchSound();
               rockPush.ActivateRB();
               hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
           OnComplete(() => {
               hand.inUse = false;
               leftHand.GetComponent<MeshRenderer>().enabled = false;
           });
           });
    }

    void RightHandMovementToRockPush(RightHand hand, RockPushing rockPush, Vector3 pos)
    {
        if (hand.inUse || stopController)
            return;
        hand.inUse = true;
        rightHandLR.positionCount = 2;
        rightHand.GetComponent<MeshRenderer>().enabled = true;
        hand.transform.DOMove(pos + new Vector3(0, 2f, -1), 0.5f).SetEase(Ease.Flash).
            OnComplete(() => {
                if (GameEssentials.instance)
                    GameEssentials.instance.shm.PlayPunchSound();
                rockPush.ActivateRB();
                hand.transform.DOLocalMove(new Vector3(0, 0.005f, 0.08f), 0.2f).SetEase(Ease.Flash).
            OnComplete(() => {
                hand.inUse = false;
                rightHand.GetComponent<MeshRenderer>().enabled = false;
            });
            });
    }


    void AnimateHands()
    {
        float y = leftHand.transform.parent.parent.position.y + 0.15f;
        rightHand.transform.parent.parent.DOMoveY(y, 2.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    public void GameOver()
    {
        stopController = true;
        leftHandLR.positionCount = 0;
        rightHandLR.positionCount = 0;
    }


}
