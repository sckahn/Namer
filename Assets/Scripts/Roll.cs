using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour
{
    Vector3 target = new Vector3();
    bool isRoll;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target == Vector3.zero) return;

        transform.position = Vector3.Lerp(transform.position, target, 0.05f);
    }

    void RollToward(Vector3 direction)
    {
        if (isRoll) return;
        direction = Vector3.Normalize(direction);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        {
            target = this.transform.position + Vector3.right;
            return;
        }

        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        {
            target = this.transform.position + Vector3.left;
            return;
        }

        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z > 0)
        {
            target = this.transform.position + Vector3.forward;
            return;
        }

        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z < 0)
        {
            target = this.transform.position + Vector3.back;
            return;
        }
    }

    IEnumerator RollStop()
    {
        yield return null;
        isRoll = true;
        yield return new WaitForSeconds(1.5f);
        isRoll = false;
    }
    IEnumerator RollRotation()
    {
        yield return null;
        float rotate = 0;
        while (rotate < 90)
        {
            rotate++;
            this.transform.rotation = Quaternion.Euler(0, rotate, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(collision.gameObject.name);
            RollToward(this.transform.position - collision.transform.position);
            StartCoroutine(RollStop());
        }
    }
}
