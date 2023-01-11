using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool isInteraction = false;
    public Rigidbody myRb;
    public Vector2 TileBound;

    public GameObject forwardObjectInfo;
    public GameObject curObjectInfo;

    public float padding = 0.2f;

    public struct FieldBounds
    {
        public Vector2 TopLeft;
        public Vector2 TopRight;
        public Vector2 BottomLeft;
        public Vector2 BottomRight;
        public Vector3 Center;
        public Vector3 Extends;
    }

    public FieldBounds fieldBounds;

    public void Start()
    {
        myRb = GetComponent<Rigidbody>();
    }

    public void CheckForwardObj()
    {
        RaycastHit IORay;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd, Color.green);

        if (Physics.Raycast(new Vector3(this.transform.position.x, this.transform.position.y + 0.2f, this.transform.position.z), fwd, out IORay, 0.2f))
        {
            if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == -1 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                Debug.Log("Right");
                forwardObjectInfo = IORay.transform.gameObject;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 1)
            {
                Debug.Log("Bottom");
                forwardObjectInfo = IORay.transform.gameObject;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 1 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                Debug.Log("Left");
                forwardObjectInfo = IORay.transform.gameObject;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == -1)
            {
                Debug.Log("Top");
                forwardObjectInfo = IORay.transform.gameObject;
            }
        }

        else
        {
            forwardObjectInfo = null;
        }
    }

    public void CheckCurrentTile()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 2f))
        {
            if (!forwardObjectInfo)
            {
                curObjectInfo = hit.transform.gameObject;
            }
        }
    }

    public void CulculateTilebound()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 2f))
        {
            var bounds = hit.transform.GetComponent<BoxCollider>().bounds;
            Vector3 fieldCenter = bounds.center;
            Vector3 extentsLength = bounds.extents;

            fieldBounds.Center = fieldCenter;
            fieldBounds.Extends = extentsLength;


            fieldBounds.TopLeft = new Vector2(fieldCenter.x - extentsLength.x, fieldCenter.z + extentsLength.z);
            fieldBounds.TopRight = new Vector2(fieldCenter.x + extentsLength.x, fieldCenter.z + extentsLength.z);
            fieldBounds.BottomLeft = new Vector2(fieldCenter.x - extentsLength.x, fieldCenter.z - extentsLength.z);
            fieldBounds.BottomRight = new Vector2(fieldCenter.x + extentsLength.x, fieldCenter.z - extentsLength.z);
        }
    }

    public void CalculateNextTile()
    {
        if (this.transform.position.z > fieldBounds.TopLeft.y - padding)
        {
            Debug.Log("UP");
        }

        else if (this.transform.position.x > fieldBounds.TopRight.x - padding)
        {
            Debug.Log("Right");
        }

        else if (this.transform.position.x < fieldBounds.TopLeft.x + padding)
        {
            Debug.Log("Left");
        }

        else if (this.transform.position.z < fieldBounds.BottomLeft.y + padding)
        {
            Debug.Log("BUTTOM");
        }
    }

    private void Update()
    {
        CheckCurrentTile();
        CheckForwardObj();

        Debug.Log(curObjectInfo);
        Debug.Log(forwardObjectInfo);


        if (Input.GetKeyDown(KeyCode.B))
        {
            isInteraction = true;
        }
    }
}
