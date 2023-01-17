using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ObjDirection
{
    Up,
    Down,
    Left,
    Right,
    Front, 
    Back

}


public class CheckSurrounding : MonoBehaviour
{
    public GameObject forwardObjectInfo;
    public GameObject curObjectInfo;
  


    public enum PlayerDirs
    {
        Top = 0,
        Bottom,
        Left,
        Right,
        Null
    }
    public PlayerDir mydir;
    private void Start()
    {
    }

    //Find All object In scene Problecm is it collects Rock also
    public List<GameObject> FindAllInteractObjectsInScene()
    {
        List<GameObject> allObjectInScene = new List<GameObject>();

        // Get all objects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        // Loop through all objects in the scene
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].tag == "InteractObj")
            {
                allObjectInScene.Add(allObjects[i]);
            }
        }
        // foreach (var item in allObjectInScene)
        // {
        //     print(item.name);
        // }

        return allObjectInScene;
    }

   
    public List<GameObject> FindNeighborsObjects(GameObject originObject, float checkDistance)
    {
        List<GameObject> newNeighbors = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        for (int i = 0; i < allObjects.Length; i++)
        {
            float distance = Vector3.Distance(originObject.transform.position, allObjects[i].transform.position);

            if (distance <= originObject.GetComponent<Renderer>().bounds.extents.magnitude + checkDistance)
            {
                newNeighbors.Add(allObjects[i]);
            }
        }

        foreach (var item in newNeighbors)
        {
            print(item.name);
        }

        return newNeighbors;
    }

    

    // check neighboring gameobject using rigidbody.sweeptest returns dictionary<object dictionary[enum], gameobject>
    public Dictionary<ObjDirection, GameObject[]> FindNeighboursObjectsUsingSweepTest(GameObject checkingObject, float sweepDistance)
    {
        sweepDistance = 1f;
         var rb = CheckRigidBody(checkingObject);
            Dictionary<ObjDirection, GameObject[]> objsDict = new Dictionary<ObjDirection, GameObject[]>();
            
            var forwardObj= rb.SweepTestAll(Vector3.forward, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Front, forwardObj);
    
            var backObj = rb.SweepTestAll(Vector3.back, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Back,backObj);
            
            var leftObj = rb.SweepTestAll(Vector3.left, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Left,leftObj);
            
            var rightObj = rb.SweepTestAll(Vector3.right, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Right,rightObj);
            
            var upObj = rb.SweepTestAll(Vector3.up, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Up,upObj);
            
            var downObj = rb.SweepTestAll(Vector3.down, sweepDistance).Select(x=>x.transform.gameObject).ToArray();
            objsDict.Add(ObjDirection.Down,downObj);
            
            return objsDict;
    }
    //CheckRigidBody() it check's rigid body and if it's not exist than make new one 
    //WARNING!!: iskinematic = ture, useGravity = false
    public Rigidbody CheckRigidBody(GameObject go)
    {
        if (go.TryGetComponent(out Rigidbody rigid)) return rigid;
        else
        {
            var rb =go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            return rb;
        }
    }

    //CheckNeighborsWithCollider() check neighboring gameobject with physics.overlapbox which requires collider 
    
    public Dictionary<ObjDirection, List<GameObject>> CheckNeighborsWithCollider(GameObject checkingGameObject)
    {
        Dictionary<ObjDirection, List<GameObject>> neigborObjectDict = new Dictionary<ObjDirection, List<GameObject>>();
        for (int i = 0; i <= (int)ObjDirection.Back; i++)
        {
            neigborObjectDict.Add((ObjDirection)i, new List<GameObject>());
        }
        var colider = checkingGameObject.GetComponent<Collider>().bounds.center;
        var colliders = Physics.OverlapBox(colider, transform.localScale);
        foreach (var item in colliders)
        {
            // right side
            if ((item.transform.position - checkingGameObject.transform.position).normalized.x > 0 &&
                (item.transform.position - checkingGameObject.transform.position).normalized.y >= 0)
            {
                neigborObjectDict[ObjDirection.Right].Add(item.gameObject);
            }
            //left side
            if ((item.transform.position - checkingGameObject.transform.position).normalized.x < 0 &&
                (item.transform.position - checkingGameObject.transform.position).normalized.y > 0)
            {
                neigborObjectDict[ObjDirection.Left].Add(item.gameObject);
            }
            //front
            if ((item.transform.position - checkingGameObject.transform.position).normalized.z > 0 &&
                (item.transform.position - checkingGameObject.transform.position).normalized.y > 0)
            {
                neigborObjectDict[ObjDirection.Front].Add(item.gameObject);
            }
            //back
            if ((item.transform.position - checkingGameObject.transform.position).normalized.z < 0 &&
                (item.transform.position - checkingGameObject.transform.position).normalized.y > 0)
            {
                neigborObjectDict[ObjDirection.Back].Add(item.gameObject);
            }
            //down
            if ((item.transform.position - checkingGameObject.transform.position).normalized.y < 0
                && (item.transform.position - checkingGameObject.transform.position).normalized.x == 0
                && (item.transform.position - checkingGameObject.transform.position).normalized.z == 0)
            {
                neigborObjectDict[ObjDirection.Down].Add(item.gameObject);
            }
            //Up
            if ((item.transform.position - checkingGameObject.transform.position).normalized.y > 0
                && (item.transform.position - checkingGameObject.transform.position).normalized.x == 0
                && (item.transform.position - checkingGameObject.transform.position).normalized.z == 0)
            {
                neigborObjectDict[ObjDirection.Up].Add(item.gameObject);
                print(item.name);
            }
        }
    
        return neigborObjectDict;
    }

    //CheckUpsideNeighboringGameObject() is used for just upward check 
    public GameObject CheckUpsideNeighboringGameObject(GameObject checkOrigin)
    {
        var extents = checkOrigin.GetComponent<Collider>().bounds.extents;
        var centerPos = checkOrigin.GetComponent<Collider>().bounds.center;
        Ray ray = new Ray(new Vector3(centerPos.x, centerPos.y + extents.y, centerPos.z), Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1f))
        {
            return hit.transform.gameObject;
        }

        return null;
    }



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
        //Debug.DrawRay(transform.position, fwd, Color.green);

        if (Physics.Raycast(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), fwd, out IORay, 0.5f))
        {
            if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == -1 &&
                curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                mydir = PlayerDir.Right;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 1)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                mydir = PlayerDir.Bottom;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 1 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == 0)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                mydir = PlayerDir.Left;
            }

            else if (curObjectInfo.GetComponent<BoxCollider>().bounds.center.x - IORay.transform.position.x == 0 &&
                     curObjectInfo.GetComponent<BoxCollider>().bounds.center.z - IORay.transform.position.z == -1)
            {
                forwardObjectInfo = IORay.transform.gameObject;
                mydir = PlayerDir.Top;
            }
        }

        else
        {
            forwardObjectInfo = null;
            mydir = PlayerDir.Null;
        }
    }

    #endregion
   
}

