using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Update = UnityEngine.PlayerLoop.Update;

public enum ObjDirection
{
    Up,
    Down,
    Left,
    Right,
    Front, 
    Back

}
public enum Dir
{
    left = 0,
    right,
    down,
    up,
    back,
    forward
}

public class CheckSurrounding : MonoBehaviour
{
    public GameObject forwardObjectInfo;
    public GameObject curObjectInfo;

    #region moho's CheckCoding
    
    
    [SerializeField] float baseSize = 1f;
    List<List<float>> dots = new List<List<float>>(3);
    bool timeToDetect = false;
    Vector3[] dirs = { Vector3.left, Vector3.right, Vector3.down, Vector3.up, Vector3.back, Vector3.forward };
    void InitArray()
    {
        dots = new List<List<float>>(3);
        dots.Add(new List<float>());
        dots.Add(new List<float>());
        dots.Add(new List<float>());
    }

    void OnChangedScaleOrPosition()
    {
        InitArray();

        float lossyScaleX = this.transform.lossyScale.x;
        int countX = (int)Math.Round(lossyScaleX / baseSize);
        float lossyScaleY = this.transform.lossyScale.y;
        int countY = (int)Math.Round(lossyScaleY / baseSize);
        float lossyScaleZ = this.transform.lossyScale.z;
        int countZ = (int)Math.Round(lossyScaleZ / baseSize);

        float floorX = transform.position.x - (lossyScaleX / 2f) - (baseSize / 2f);
        //float floorX = transform.position.x - 0.5f;
        for (int i = 0; i < countX; i++)
        {
            dots[0].Add(floorX + i + 1);
        }

        // 해당 타일은 position 위치가 땅바닥에 있으므로 따로 스케일 값을 뺄 필요가 없음 
        //float floorY = transform.position.y - (lossyScaleY / 2f) - 0.5f;
        float floorY = transform.position.y - (baseSize / 2f);
        for (int i = 0; i < countY; i++)
        {
            dots[1].Add(floorY + i + 1);
        }

        float floorZ = transform.position.z - (lossyScaleZ / 2f) - (baseSize / 2f);
        //float floorZ = transform.position.z - 0.5f;
        for (int i = 0; i < countZ; i++)
        {
            dots[2].Add(floorZ + i + 1);
        }
    }

    // 검출 코드 --> Transform hit을 반환 
    Transform Detect(float x, float y, float z, Dir dir)
    {
        RaycastHit[] hit = Physics.RaycastAll(new Vector3(x, y, z) - dirs[(int)dir] * 0.2f, dirs[(int)dir], (baseSize))
            .OrderBy(h => h.distance)
            .Where(h => /*h.transform.CompareTag("InteractObj") &&*/ h.transform != this.transform).ToArray();
        Debug.DrawRay(new Vector3(x, y, z) - dirs[(int)dir] * 0.2f, dirs[(int)dir], Color.blue, (baseSize / 2f));
        if (hit.Length != 0)
        {
            foreach (RaycastHit obj in hit)
                Debug.Log("Hit! " + obj.transform.name + " / dir : " + dir.ToString());

            return hit[0].transform;
        }
        return null;
    }

    // 원하는 한 방향 검출하기 --> List<Transform> hits를 반환 
    public List<Transform> GetTransformsAtDirOrNull(Dir dir)
    {
        List<Transform> hitObjs = new List<Transform>();
        switch ((int)dir)
        {
            case (0):
            case (1):
                for (int i = 0; i < dots[1].Count; i++)
                {
                    for (int l = 0; l < dots[2].Count; l++)
                    {
                        Transform t = Detect(transform.position.x + (transform.lossyScale.x / 2f) * ((int)dir == 0 ? -1 : 1), dots[1][i], dots[2][l], dir);
                        if (t != null)
                            hitObjs.Add(t);
                    }
                }
                break;
            case (2):
            case (3):
                for (int i = 0; i < dots[0].Count; i++)
                {
                    for (int l = 0; l < dots[2].Count; l++)
                    {
                        // y축은 기본 position이 바닥에 붙어있으므로 바닥위치는 구할 필요 없고, 천장 위치만 스케일을 더해서 계산
                        Transform t = Detect(dots[0][i], transform.position.y + (transform.lossyScale.y) * ((int)dir - 2), dots[2][l], dir);
                        if (t != null)
                            hitObjs.Add(t);
                    }
                }
                break;
            case (4):
            case (5):
                for (int i = 0; i < dots[0].Count; i++)
                {
                    for (int l = 0; l < dots[1].Count; l++)
                    {
                        Transform t = Detect(dots[0][i], dots[1][l], transform.position.z + (transform.lossyScale.z / 2f) * ((int)dir == 4 ? -1 : 1), dir);
                        if (t != null)
                            hitObjs.Add(t);
                    }
                }
                break;
            default:
                break;
        }
        return hitObjs.Count == 0 ? null : hitObjs;
    }

    // 6방향 모두 검출하기 --> List<Transform> hits를 반환 
    public List<Transform> GetTransformsAroundObjectOrNull()
    {
        List<Transform> result = new List<Transform>();

        // 6방향으로 검출 
        for (int i = 0; i < 6; i++)
        {
            List<Transform> transforms = GetTransformsAtDirOrNull((Dir)i);
            bool isNull = (transforms == null);
            if (!isNull)
            {
                foreach (Transform t in transforms)
                    if (t != null) result.Add(t);
            }
        }

        //SetTimeTrue();
        return result.Count == 0 ? null : result;
    }

    

    #endregion
  

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

      
        return newNeighbors;
    }

    

    // check neighboring gameobject using rigidbody.sweeptest returns dictionary<object dictionary[enum], gameobject>
    public Dictionary<ObjDirection, GameObject[]> CheckNeighboursObjectsUsingSweepTest(GameObject checkingObject, float sweepDistance)
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
        var colliders = Physics.OverlapBox(colider, transform.localScale/2);
        foreach (var item in colliders)
        {
            if(item.gameObject == null) continue;
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

