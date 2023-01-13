using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody rb;
    public float currentSpeed;
    public Animator myanimator;
    public bool isInteraction = false;
    public GameObject interactobj;

    private void Start()
    {
        myanimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //TODO 중력가속도 개선 
        if (!isInteraction)
        {
            myanimator.SetBool("isInteraction", false);
            Vector3 pVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            rb.velocity = moveSpeed * pVelocity;

            if (pVelocity != Vector3.zero)
            {
                rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(pVelocity), Time.deltaTime * 10f);
            }

            if (rb.velocity.magnitude > 0f)
            {
                myanimator.SetBool("isRun", true);
                myanimator.SetBool("TakeItem", false);
                myanimator.SetBool("InteractObj", false);
                myanimator.SetBool("isVictory", false);
            }

            else
            {
                myanimator.SetBool("isRun", false);
                myanimator.SetBool("TakeItem", false);
                myanimator.SetBool("InteractObj", false);
                myanimator.SetBool("isVictory", false);
            }
        }

        if (TryGetComponent<PlayerInteraction>(out PlayerInteraction pi) &&
            pi.forwardObjectInfo  &&
            pi.forwardObjectInfo.name == "GoalPointObj")
        {
            Debug.Log("h");
            myanimator.SetBool("isInteraction", true);
            myanimator.SetBool("isVictory", true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayPushAnimation();
        }

        isInteraction = myanimator.GetCurrentAnimatorStateInfo(0).IsName("takeitem") |
            myanimator.GetCurrentAnimatorStateInfo(0).IsName("push") |
            myanimator.GetCurrentAnimatorStateInfo(0).IsName("victory") &&
            myanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? true : false;

    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.transform.name == "Goal")
    //    {
    //        myanimator.SetBool("isInteraction", true);
    //        myanimator.SetBool("isVictory", true);
    //    }

    //    isInteraction = myanimator.GetCurrentAnimatorStateInfo(0).IsName("takeitem") |
    //myanimator.GetCurrentAnimatorStateInfo(0).IsName("push") |
    //myanimator.GetCurrentAnimatorStateInfo(0).IsName("victory") &&
    //myanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? true : false;
    //}

    public void PlayPushAnimation1()
    {
        StartCoroutine(AnimationPlay());    
    }

    public void PlayPushAnimation()
    {
        myanimator.SetBool("isInteraction", true);

        if (TryGetComponent<PlayerInteraction>(out PlayerInteraction pi) && pi.forwardObjectInfo)
        {
            if (pi.forwardObjectInfo.tag == "Item")
            {
                myanimator.SetBool("TakeItem", true);
            }

            else if (pi.forwardObjectInfo.tag == "InteractObj")
            {
                interactobj = pi.forwardObjectInfo;
                myanimator.SetBool("InteractObj", true);
                pi.forwardObjectInfo.GetComponent<ObjectClass>().Interact(this.gameObject);
            }

            else if (pi.forwardObjectInfo.name == "Goal")
            {
                myanimator.SetBool("isVictory", true);
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

