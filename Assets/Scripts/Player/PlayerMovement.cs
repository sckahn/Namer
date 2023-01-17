using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region components
    private Rigidbody rb;
    private Animator myanimator;
    private PlayerEntity playerEntity;
    #endregion

    public bool isInteraction = false;
    public GameObject interactobj;

    public float moveSpeed;
    public int rotateSpeed;
    //private float curVelocityY;

    public bool _canPlayerInput = true;

    // TODO KeyMapping

    private void Start()
    {
        myanimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerEntity = GetComponent<PlayerEntity>();
    }

    private void PlayerMove(Vector3 inputVec)
    {
        //Player Move
        float gravity = rb.velocity.y; 

        Vector3 velocity = moveSpeed * new Vector3(inputVec.x, 0, inputVec.z);
        velocity.y = gravity;
        rb.velocity = velocity;

        //Player Rotation
        rb.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * rotateSpeed);
    }
        
    private Vector3 PlayerInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void Update()
    {
        // TODO GameManager한테 인풋 받을 수 있는 변수 가져오기
        if (_canPlayerInput && !playerEntity.doInteraction)
        {
            // 이동 함수 + 인터렉션
            PlayerMove(PlayerInput());

            if (Input.GetKeyDown(playerEntity.interactionKey))
            {
                if (TryGetComponent<PlayerInteraction>(out PlayerInteraction pi1) && pi1.forwardObjectInfo)
                {
                    if (pi1.forwardObjectInfo.tag == "InteractObj")
                    {
                        interactobj = pi1.forwardObjectInfo;
                        playerEntity.ChangeState(PlayerStates.Push);
                        pi1.forwardObjectInfo.GetComponent<ObjectClass>().Interact(this.gameObject);
                    }
                }
            }
        }

        //if (TryGetComponent<PlayerInteraction>(out PlayerInteraction pi) &&
        //    pi.forwardObjectInfo &&
        //    pi.forwardObjectInfo.name == "GoalPointObj")
        //{
        //    //myanimator.SetBool("isInteraction", true);
        //    //myanimator.SetBool("isVictory", true);
        //}

        //if (!isInteraction)
        //{
        //    myanimator.SetBool("isInteraction", false);

        //    if (rb.velocity.magnitude > 0f)
        //    {
        //        myanimator.SetBool("TakeItem", false);
        //        myanimator.SetBool("InteractObj", false);
        //        myanimator.SetBool("isVictory", false);
        //    }

        //    else
        //    {
        //        myanimator.SetBool("isRun", false);
        //        myanimator.SetBool("TakeItem", false);
        //        myanimator.SetBool("InteractObj", false);
        //        myanimator.SetBool("isVictory", false);
        //    }
        //}




        //isInteraction = myanimator.GetCurrentAnimatorStateInfo(0).IsName("takeitem") |
        //    myanimator.GetCurrentAnimatorStateInfo(0).IsName("push") |
        //    myanimator.GetCurrentAnimatorStateInfo(0).IsName("victory") &&
        //    myanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? true : false;

    }

    public void PlayPushAnimation1()
    {
        StartCoroutine(AnimationPlay());    
    }

    public void PlayPushAnimation()
    {

        if (TryGetComponent<PlayerInteraction>(out PlayerInteraction pi) && pi.forwardObjectInfo)
        {
            if (pi.forwardObjectInfo.tag == "InteractObj")
            {
                interactobj = pi.forwardObjectInfo;
                playerEntity.ChangeState(PlayerStates.Push);
                pi.forwardObjectInfo.GetComponent<ObjectClass>().Interact(this.gameObject);
            }
        }
    }

    Vector3 CalculateTargetPos()
    {
        PlayerInteraction pi = GetComponent<PlayerInteraction>();

        Vector3 targetPos = new Vector3();

        if ((int)pi.mydir == 0)
        {
            targetPos += Vector3.forward;
        }

        else if ((int)pi.mydir == 3)
        {
            targetPos += Vector3.right;
        }

        else if ((int)pi.mydir == 1)
        {
            targetPos += Vector3.back;
        }

        else if ((int)pi.mydir == 2)
        {
            targetPos += Vector3.left;
        }

        return targetPos;
    }

    IEnumerator AnimationPlay()
    {
        Vector3 targetPos = CalculateTargetPos();
        float padding = 0.005f;
        Vector3 temp = targetPos + interactobj.transform.position;

        while ((interactobj.transform.position - temp).magnitude > 0.01f)
        {
            interactobj.transform.position = new Vector3(targetPos.x * padding + interactobj.transform.position.x,
                interactobj.transform.position.y,
                targetPos.z * padding + interactobj.transform.position.z);

            this.transform.position = new Vector3(targetPos.x * padding + this.transform.position.x,
             this.transform.position.y,
             targetPos.z * padding + this.transform.position.z);

            yield return new WaitForSeconds(0.01f);
        }
        //TODO 포지션 맞춰주기

        interactobj.transform.position = new Vector3(Mathf.Round(interactobj.transform.position.x), Mathf.Round(interactobj.transform.position.y), Mathf.Round(interactobj.transform.position.z));
    }
}

