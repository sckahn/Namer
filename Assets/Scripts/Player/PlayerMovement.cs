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

        

        if (Input.GetKeyDown(KeyCode.B))
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
                    pi.forwardObjectInfo.GetComponent<ObjectClass>().Interact(this.gameObject);
                    interactobj = pi.forwardObjectInfo;
                    myanimator.SetBool("InteractObj", true);

                }

                else if (pi.forwardObjectInfo.name == "Goal")
                {
                    myanimator.SetBool("isVictory", true);
                }
            }
        }

        isInteraction = myanimator.GetCurrentAnimatorStateInfo(0).IsName("takeitem") |
            myanimator.GetCurrentAnimatorStateInfo(0).IsName("push") |
            myanimator.GetCurrentAnimatorStateInfo(0).IsName("victory") &&
            myanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? true : false;

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.name == "Goal")
        {
            myanimator.SetBool("isInteraction", true);
            myanimator.SetBool("isVictory", true);
        }

        isInteraction = myanimator.GetCurrentAnimatorStateInfo(0).IsName("takeitem") |
    myanimator.GetCurrentAnimatorStateInfo(0).IsName("push") |
    myanimator.GetCurrentAnimatorStateInfo(0).IsName("victory") &&
    myanimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 ? true : false;
    }

    public void PlayPushAnimation()
    {
        StartCoroutine(AnimationPlay());    
    }

    IEnumerator AnimationPlay()
    {
        while(interactobj.transform)
        {


            yield return new WaitForSeconds(0.1f);
        }
    }
}

