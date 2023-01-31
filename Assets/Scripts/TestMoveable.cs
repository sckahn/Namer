using System.Collections;
using UnityEngine;

public class TestMoveable : MonoBehaviour
{
    private EAdjective adjectiveName = EAdjective.Movable;
    private EAdjectiveType adjectiveType = EAdjectiveType.Normal;
    private int count = 0;

    private float currentTime;
    private bool isRoll;
    private int movingSpeed = 1;
    Vector3 target;

    public void StartMove()
    {
        if (isRoll) return;
        
        Vector3 direction = (this.transform.position - GameObject.FindWithTag("Player").transform.position);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x > 0)
        {
            if (DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.gameObject, Dir.right) != null ) return;

            target = this.transform.position + Vector3.right;

            this.StartCoroutine(MoveObj(this.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z) && direction.x < 0)
        {
            if (DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.gameObject, Dir.left) != null) return;

            target = this.transform.position + Vector3.left;

            this.StartCoroutine(MoveObj(this.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z > 0)
        {
            if (DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.gameObject, Dir.forward) != null) return;

            target = this.transform.position + Vector3.forward;

            this.StartCoroutine(MoveObj(this.gameObject));

            return;
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.z) && direction.z < 0)
        {
            if (DetectSurroundingHS.GetInstance.GetAdjacentObjectWithDir(this.gameObject, Dir.back) != null) return;
                
            target = this.transform.position + Vector3.back;

            this.StartCoroutine(MoveObj(this.gameObject));

            return;
        }
    }
    
    IEnumerator MoveObj(GameObject obj)
    {
        currentTime = 0;
        Vector3 startPos = obj.transform.localPosition;
        isRoll = true;

        InteractionMaster.GetInstance.isPause = true;

        while (currentTime < movingSpeed)
        {
            currentTime += Time.deltaTime;
            obj.transform.localPosition = Vector3.Lerp(startPos, target, currentTime / movingSpeed);

            yield return new WaitForFixedUpdate();
        }
        isRoll = false;
        
        InteractionMaster.GetInstance.isPause = false;

    }
}


