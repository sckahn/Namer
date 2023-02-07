using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DetectManager : Singleton<DetectManager>
{
    public GameObject forwardObjectInfo;
    public GameObject curObjectInfo;
    public Dir objDir;
    
    
#region CharacterCheck
    //CheckCharacterCurrentTile() it cast a ray to buttom of an GameObject
    public void CheckCharacterCurrentTile(GameObject character)
    {
        RaycastHit hit;
        if (Physics.Raycast(character.transform.position, Vector3.down, out hit, 2f))
        {
            if (!forwardObjectInfo)
            {
                curObjectInfo = hit.transform.gameObject;
            }
        }
    }

    //CheckForwardObj() it checks what is infront of gameobject
    public void CheckForwardObj(GameObject gameObject)
    {
        RaycastHit IORay;
        Vector3 fwd = gameObject.transform.TransformDirection(Vector3.forward);
        var position = gameObject.transform.position;
        //Debug.DrawRay(new Vector3(position.x, position.y + 0.5f, position.z), fwd, Color.green);

        if (Physics.Raycast(new Vector3(position.x, position.y + 0.5f, position.z), fwd, out IORay, 0.2f))
        {
            if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == -1 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                objDir = Dir.right;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 1)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                objDir = Dir.down;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 1 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                objDir = Dir.left;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == -1)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                objDir = Dir.up;
            }
        }

        else
        {
            forwardObjectInfo = null;
            objDir = Dir.Null;
        }
    }

    #endregion


}
