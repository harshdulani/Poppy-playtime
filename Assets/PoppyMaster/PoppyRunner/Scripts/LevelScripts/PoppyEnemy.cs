using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public enum PoppyEnemyState {None, Idle, MoveForward, CarryUp, Run, ClimbUp, ClimbToRoof, CarryJump };
public class PoppyEnemy : MonoBehaviour
{
    public PoppyEnemyState myState;
    public bool run;
    public bool gotHit, move, climb;
    public int blendShapeWeight;
    public GameObject girl;
    public Transform wrist;
    public float baseScale, movingSpeed;
    public List<Rigidbody> bodyParts;
    public List<GameObject> faces;
    public Animator myAnimator { get { return GetComponent<Animator>(); } }
    public SkinnedMeshRenderer newEnemy;

    Transform myTransform { get{ return transform; } }
    PlayerController playerController { get { return PlayerController.instance; } }

    bool StillCarrying;

    private void Awake()
    {
        girl.tag = "Untagged";
        RB_State(true);
    }

    private void Start()
    {
        if (myState == PoppyEnemyState.MoveForward)
            myAnimator.SetTrigger("Run");
        

        if (newEnemy)
            newEnemy.SetBlendShapeWeight(0,blendShapeWeight);
    }
    public void RB_State(bool state)
    {
        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].isKinematic = state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand"))
        {
            if (myState == PoppyEnemyState.ClimbToRoof || myState == PoppyEnemyState.CarryJump)
                return;

            print("Ragdoll");
            ActivateRB(other.transform);
            FaceChange(1);
            if (GameEssentials.instance)
                GameEssentials.instance.shm.Invoke("PlayEnemySound", 0.25f);
        }

        if (other.name.Contains("Cube"))
        {
            if (PlayerController.instance.stopController)
                return;

            if (gotHit)
                return;
            gotHit = true;
            ActivateRB(other.transform);
            FaceChange(1);
            if (GameEssentials.instance)
                GameEssentials.instance.shm.Invoke("PlayEnemySound", 0.25f);
            
        }

        if (other.name.Contains("barrel"))
        {
            if (PlayerController.instance.stopController)
                return;

            if (gotHit)
                return;
            gotHit = true;
            FindObjectOfType<Bridge>().BlastBarrel(other.gameObject);
            ThrowSideWays();
            FaceChange(1);
            if (GameEssentials.instance)
                GameEssentials.instance.shm.Invoke("PlayEnemySound", 0.25f);
           
        }

        if (other.name.Contains("Rotation"))
        {
             if (run)
                return;
         
            myTransform.DORotate(new Vector3(0, 180, 0), 0.25f).SetEase(Ease.Flash);
        }

        if (other.name.Contains("ClimbUp"))
        {
            print("ClimUp");
            StartCoroutine("ClimbUpAndAttack");
        }
    }

    IEnumerator ClimbUpAndAttack()
    {
        if (gotHit)
            yield return null;

        GetComponent<Collider>().enabled = false;
        myState = PoppyEnemyState.ClimbToRoof;
        climb = false;
        myAnimator.SetTrigger("Top");
        yield return new WaitForSeconds(4f);
        Invoke("PlayerDrop", 0.2f);
        if (!playerController.girlCaptured)
        {
            StillCarrying = true;
            myState = PoppyEnemyState.CarryJump;
            playerController.girlCaptured = true;
          //  myTransform.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.Flash);
        }
    }

    void PlayerDrop()
    {
        if (gotHit || playerController.stopController)
            return;
        playerController.GameOver();
    }


    public void ThrowSideWays()
    {
        gotHit = true;
        RB_State(false);
        myAnimator.enabled = false;
        
        GetComponent<Collider>().enabled = false;
        if (myTransform.position.x > 0)
            bodyParts[0].AddForce(Vector3.right * 350, ForceMode.Impulse);
        else
            bodyParts[0].AddForce(Vector3.left * 350, ForceMode.Impulse);

        //if(myTransform.position.x>0)
        //    bodyParts[0].AddForce(-myTransform.forward + new Vector3(1.5f,0,0) * 350, ForceMode.Impulse);
        //else
        //    bodyParts[0].AddForce(-myTransform.forward + new Vector3(1.5f,0,-1) * 350, ForceMode.Impulse);
    }

    public void ActivateRB(Transform hand)
    {
        if (myState == PoppyEnemyState.ClimbToRoof || myState == PoppyEnemyState.CarryJump)
            return;

        myAnimator.enabled = false;
        RB_State(false);
        gotHit = true;
        GetComponent<Collider>().enabled = false;
        Vector3 dir = hand.position - myTransform.position;

        bodyParts[0].AddForce(-dir.normalized + new Vector3(myTransform.position.normalized.x, 1.5f, 25f) * 150, ForceMode.Impulse);

        if (FallenBridge.instance)
        {
           
            for (int i = 0; i < FallenBridge.instance.temp.Length; i++)
            {
                FallenBridge.instance.temp[i].ActivateGorillas();
            }

            FallenBridge.instance.CheckForBridgesCleared();
        }

        if (GameEssentials.instance.sl.GetCurrentSceneByName().Contains("BuildingTower"))
        {
            if(PoppyEnemies.instance.IsPoppiesCleared())
                UIManager.instance.LevelCompleted();
        }

        //if (myTransform.position.x > 0)
        //    bodyParts[0].transform.DOLocalMove( new Vector3(2f, 2f, 8f), 0.5f).SetEase(Ease.Flash);
        //else if (myTransform.position.x < 0)
        //    bodyParts[0].transform.DOLocalMove( new Vector3(-2f, 2f, 8f), 0.5f).SetEase(Ease.Flash);
        //else
        //    bodyParts[0].transform.DOLocalMove( new Vector3(0f, 2f, 8f), 0.5f).SetEase(Ease.Flash);
    }

    public void LiftUp()
    {
        StillCarrying = false;
        move = false;
        playerController.GameOver();
        myTransform.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.Flash);
        myTransform.DOMove(playerController.girl.transform.position + new Vector3(0, 0, 6.5f), 0.25f).SetEase(Ease.Flash).OnComplete(()=> { myAnimator.SetTrigger("Lift"); Invoke("SetGirlAsParent", 1.2f); });
        if (Help.instance)
            Help.instance.gameObject.SetActive(false);
    }

    public void ClimbUpLiftUp()
    {
        move = false;
        gotHit = false;
        StillCarrying = false;
        playerController.GameOver();
       // myTransform.DOScale(Vector3.one * baseScale, 0.25f).SetEase(Ease.Flash);
        float y = 15.5f;
        Vector3 tempPos;
        if (name == "Gorilla")
        {
            y = 13f;
            tempPos = myTransform.GetChild(1).position;
        }
        else 
        {
          tempPos = myTransform.GetChild(0).GetChild(0).GetChild(1).position;
        }

        myAnimator.SetTrigger("Lift");

        myTransform.DOMove(new Vector3(tempPos.x, y, tempPos.z), 0.01f).SetEase(Ease.Flash);
        GameObject.Find("CoverUp").GetComponent<ParticleSystem>().Play();
        Invoke("SetGirlAsParent",1.2f);
        
        if (Help.instance)
            Help.instance.gameObject.SetActive(false);
    }

    void SetGirlAsParent()
    {
        playerController.girl.transform.SetParent(wrist);
      
        if (GameEssentials.instance)
            GameEssentials.instance.shm.PlayGirlScreamSound();
        playerController.girl.transform.DOLocalMove(girl.transform.position, 0.1f).SetEase(Ease.Flash).OnComplete(()=>{ 
            playerController.girl.transform.DORotate(girl.transform.eulerAngles, 0.1f).SetEase(Ease.Flash).OnComplete(() => { 
                girl.SetActive(true); 
                playerController.girl.SetActive(false);
                girl.GetComponent<Animator>().SetTrigger("Walk");
                myTransform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Flash).OnComplete(()=> {
                    if (myState == PoppyEnemyState.CarryUp)
                    {
                        run = true;
                        myState = PoppyEnemyState.Run;
                        myAnimator.SetTrigger("Run");
                        PoppyEnemies.instance.RunBack();
                    }
                    else if (myState == PoppyEnemyState.CarryJump)
                    {
                        print("Jump");

                        myTransform.DOMoveY(myTransform.position.y + 8f, 0.5f).SetEase(Ease.Flash).SetDelay(2f).OnComplete(() =>
                        {
                            myAnimator.enabled = false;
                            myTransform.DOMoveZ(myTransform.position.z + 75f, 1.25f);
                            myTransform.DOMoveX(myTransform.position.x + 75f, 1.25f);
                            myTransform.DOMoveY(-200f, 3f);
                        });
                    }
                   
                    UIManager.instance.LevelFailed();
                });
            }); });
        
    }

    public void RunBack()
    {
        if (gotHit)
            return;

        myTransform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Flash).OnComplete(() => {
            run = true;
            move = false;
            myAnimator.SetTrigger("Run");  
        });
    }

    private void Update()
    {
        if (gotHit)
            return;

        StateController();
        //MoveForward();
        //MoveUp();
    }

    void MoveUp()
    {
        if (climb)
        {
            myTransform.position += myTransform.up * movingSpeed * Time.deltaTime;
            return;
        }
    }

    void MoveForward()
    {
        if (run) { 
            move = false;
            myTransform.position += Vector3.forward * 20 * Time.deltaTime; 
        }

        if (playerController.stopController || gotHit)
            return;

        if (move)
            myTransform.position += myTransform.forward * movingSpeed * Time.deltaTime;

        if (Vector3.Distance(myTransform.position, playerController.girl.transform.position) <= 20f)
        {
            LiftUp();
        }
    }

    void StateController()
    {
        switch (myState)
        {
            case PoppyEnemyState.None:
                break;
            case PoppyEnemyState.Idle:
                if (Vector3.Distance(myTransform.position, playerController.girl.transform.position) <= 20f)
                {
                    myState = PoppyEnemyState.CarryUp;
                    StillCarrying = true;
                }
                break;
            case PoppyEnemyState.MoveForward:
                myTransform.position += myTransform.forward * movingSpeed * Time.deltaTime;
                if (Vector3.Distance(myTransform.position, playerController.girl.transform.position) <= 20f)
                {
                    myState = PoppyEnemyState.CarryUp;
                    StillCarrying = true;
                }
                break;
            case PoppyEnemyState.CarryUp:
                if(StillCarrying)
                    LiftUp();
                break;
            case PoppyEnemyState.Run:
                myTransform.position += Vector3.forward * 20 * Time.deltaTime;
                break;
            case PoppyEnemyState.ClimbUp:
                myTransform.position += myTransform.up * movingSpeed * Time.deltaTime;
                break;
            case PoppyEnemyState.ClimbToRoof:
                break;
            case PoppyEnemyState.CarryJump:
                if (StillCarrying)
                    ClimbUpLiftUp();
                break;
        }
    }    

    public void ScaleABit()
    {
        myTransform.DOScale(Vector3.one * 6.5f, 0.1f).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo);
    }

    public void FaceChange(int val)
    {
        if (faces.Count < 0)
            return;

        for (int i = 0; i < faces.Count; i++)
        {
            if (i == val)
                faces[i].SetActive(true);
            else
                faces[i].SetActive(false);
        }
    }
}
