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
        Vector3 pVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.velocity = moveSpeed * pVelocity;

        if(rb.velocity.magnitude > 2f)
        {
            myanimator.SetBool("isRun", true);
        }

        else
        {
            myanimator.SetBool("isRun", false);

        }
    }

}

