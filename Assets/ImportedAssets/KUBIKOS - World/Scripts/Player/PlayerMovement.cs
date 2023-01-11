using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody rb;
    public float currentSpeed;
    public Animator myanimator;


    private void Start()
    {
        myanimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //TODO 중력가속도 개선 

        Vector3 pVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.velocity = moveSpeed * pVelocity;

        if (pVelocity != Vector3.zero)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(pVelocity), Time.deltaTime * 10f);
        }

        if (rb.velocity.magnitude > 0f)
        {
            myanimator.SetBool("isRun", true);
        }

        else
        {
            myanimator.SetBool("isRun", false);

        }
    }

}

