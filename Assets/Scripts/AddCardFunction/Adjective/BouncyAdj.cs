using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BouncyAdj : IAdjective
{
    private EAdjective adjectiveName = EAdjective.Bouncy;
    private EAdjectiveType adjectiveType = EAdjectiveType.Repeat;
    private int count = 0;

    #region 통통 꾸밈 카드 멤버변수
    private bool isBouncy;      // 꾸밈 부여 여부 
    float bounceSpeed = 3f;     // addValue의 증감 빠르기 
    float bounciness = 4f;      // lerp 이동의 빠르기 

    int bouncyDir = 1;          // 위아래 bouncy 이동 방향 
    float addValue = 0;         // 이동 정도

    private Transform player;   // 플레이어 transform 
    private float playerRadius; // 플레이어 capsule collider의 반지름 값 
    Vector3[] playerTiles;      // 플레이어가 차지하고 있는 공간들 
    #endregion

    // todo Test를 위함이 아니라면, 반드시 Start의 내용을 지워주세요 
    private void Start()
    {
        //StartCoroutine(BounceObj(gameObject));
    }

    public EAdjective GetAdjectiveName()
    {
        return adjectiveName;
    }

    public EAdjectiveType GetAdjectiveType()
    {
        return adjectiveType;
    }

    public int GetCount()
    {
        return count;
    }

    public void SetCount(int addCount)
    {
        this.count += addCount;
    }

    public void Execute(InteractiveObject thisObject)
    {
        //thisObject.StartCoroutine(BounceObj(thisObject.gameObject));
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(BounceObj(thisObject.gameObject));
    }

    public void Execute(InteractiveObject thisObject, GameObject player)
    {
        //Debug.Log("Bouncy : this Object -> Player");
    }

    public void Execute(InteractiveObject thisObject, InteractiveObject otherObject)
    {
        //Debug.Log("Bouncy : this Object -> other Object");
    }

    public void Abandon(InteractiveObject thisObject)
    {
        InteractionSequencer.GetInstance.CoroutineQueue.Enqueue(AbandonBouncy(thisObject));
    }

    public IAdjective DeepCopy()
    {
        return new BouncyAdj();
    }

    // 해당 방향에 (몇 간격까지) 블럭이 존재하는지 체크하는 메서드 
    bool CheckExistBlock(GameObject thisObj, Dir dir, int length = 0)
    {
        // detectManager의 메서드를 이용해서 해당 방향에 존재하는 타일, 오브젝트를 검출
        // 일단 y축으로만 검사 가능하도록 만들어 놓았음
        GameObject checkedObj;
        if (dir == Dir.up)
            checkedObj = DetectManager.GetInstance.GetAdjacentObjectWithDir(thisObj, dir, (int)thisObj.transform.lossyScale.y + length);
        else
            checkedObj = DetectManager.GetInstance.GetAdjacentObjectWithDir(thisObj, dir, 1);

        // null이 아니면, true
        // null이면, false
        if (checkedObj != null)
        {
            return true;
        }
        return false;
    }

    // 해당 방향에 플레이어가 존재하는지 체크하는 메서드
    // 추가로 플레이어가 차지하고 있는 공간을 playerTiles로 저장한다.
    bool CheckExistPlayer(GameObject thisObj, Dir dir)
    {
        // 플레이어의 위치값과 플레이어의 콜라이더 반지름을 가지고,
        // x, z축으로 어디서부터 어디까지 차지하고 있는지 min, max값으로 저장 
        Vector3 playerPos = player.position;
        // 타일 배열을 초기화 (플레이어가 차지하는 공간은 최대 4칸 : 점프가 없으므로)
        playerTiles = new Vector3[4];
        float minX = playerPos.x - playerRadius + 0.5f;
        float maxX = playerPos.x + playerRadius + 0.5f;
        float minZ = playerPos.z - playerRadius + 0.5f;
        float maxZ = playerPos.z + playerRadius + 0.5f;

        // 위의 값을 통해서 차지하는 타일을 playerTiles에 저장 
        int idx = 0;
        for (int i = Mathf.FloorToInt(minX); i <= maxX; i++)
        {
            for (int j = Mathf.FloorToInt(minZ); j <= maxZ; j++)
            {
                playerTiles[idx] = new Vector3(i, Mathf.FloorToInt(playerPos.y), j);
                idx++;
            }
        }

        // 검사할 위치의 좌표를 구함 
        Vector3 objPos = thisObj.transform.position;
        Vector3 targetPos;

        // dir값에 따라 오브젝트 위치에서 각 방향의 좌표를 저장 
        switch (dir)
        {
            case (Dir.up):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.up;
                break;
            case (Dir.down):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.down;
                break;
            case (Dir.forward):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.forward;
                break;
            case (Dir.back):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.back;
                break;
            case (Dir.right):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.right;
                break;
            case (Dir.left):
                targetPos = Vector3Int.RoundToInt(objPos) + Vector3.left;
                break;
            // 위치값을 잘못 넣었거나 안 넣으면, 무조건 false
            default:
                return false;
        }

        // playerTiles에 해당 좌표값이 있다면, 플레이어가 해당 지점에 있다는 것으로 true
        // 없으면, 플레이어가 해당 지점에 없다는 것으로 false
        return playerTiles.Contains(targetPos);
    }

    private void TryBouncy(GameObject obj)
    {
        // 아래에 무언가가 있는지 체크 
        if (CheckExistPlayer(obj, Dir.down) || CheckExistBlock(obj, Dir.down))
        {
            // 위에 오브젝트가 있는지 체크
            // 위가 안 막혀있다면...
            if (CheckExistPlayer(obj, Dir.up) ? !CheckExistBlock(obj, Dir.up, 1) : !CheckExistBlock(obj, Dir.up))
            {
                // 위로(bouncyDir = 1) 이동하기 
                bouncyDir = 1;
                // 최대 위로 1칸까지 이동하기 때문에 다시 0으로 초기화 
                addValue = 0;
                // 먼저 배열을 이동시켜서 위로 이동 중에 다른 블럭이 이동해 배열에 두 오브젝트가 겹치는 현상 방지
                DetectManager.GetInstance.SwapBlockInMap(Vector3Int.RoundToInt(obj.transform.position), Vector3Int.RoundToInt(obj.transform.position) + Vector3.up);
            }
            // 천장이 오브젝트로 막혀있다면...
            else
            {
                // 움직이지 말고 그 상태로 대기 
                bouncyDir = 0;
                // 어차피 0이라 이동이 되지 않고, 언제 위나 아래가 빠질지 모르므로
                // addValue를 1로 유지 (1 이상이면 계속 탐색이 들어감)
                addValue = 1;
            }
        }
        else
        {
            // 아래에 아무 것도 없으면, 무조건 아래로 떨어지게 됨
            bouncyDir = -1;
            addValue = 0;
            DetectManager.GetInstance.SwapBlockInMap(Vector3Int.RoundToInt(obj.transform.position), Vector3Int.RoundToInt(obj.transform.position) + Vector3.down);
        }
    }

    private IEnumerator BounceObj(GameObject obj)
    {
        // 즉시 rigidBody을 kinematic 해서 중력 영향 없이 1칸 뛸 수 있도록 만듦
        var rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        player = GameObject.Find("Player").transform;
        playerRadius = player.lossyScale.x * player.GetComponent<CapsuleCollider>().radius;

        // 변수 초기화 
        addValue = 0;
        bouncyDir = 1;
        isBouncy = true;

        while (obj.GetComponent<InteractiveObject>().CheckAdjective(EAdjective.Float))
        {
            yield return new WaitForEndOfFrame();
        }

        // 처음부터 bounce가 안 되는 상황일 수 있으므로 TryBouncy로 체크 후 bounce 시작 
        TryBouncy(obj);

        // Abandon() 전까지는 계속 bounce를 하게 됨 
        while (isBouncy)
        {
            // obj가 사라지는 경우 동작 정지 
            if (obj == null)
            {
                isBouncy = false;
                //DetectManager.GetInstance.ChangeValueInMap(Vector3Int.RoundToInt(obj.transform.position), null);
                yield return null;
            }
            else if (!obj.GetComponent<InteractiveObject>().CheckAdjective(EAdjective.Bouncy))
            {
                isBouncy = false;
                yield return null;
            }

            if (!obj.GetComponent<InteractiveObject>().CheckAdjective(EAdjective.Float))
            {
                // 위로 혹은 아래로 1칸만큼 도달했을 때에 검출 시작
                // (= 배열로 먼저 이동시킨 지점에 도달했을 때)
                if (addValue >= 1)
                {
                    // 정확히 1칸을 맞추기 위해서 RoundToInt로 위치 조절
                    obj.transform.position = Vector3Int.RoundToInt(obj.transform.position);
                    // 가장 최고점 높이에 도달한 경우에는 플레이어가 들어갈 시간을 잠깐 줌 
                    if (bouncyDir == 1)
                    {
                        yield return new WaitForSeconds(0.3f);
                    }

                    // 검출 후 bounce 시작 
                    TryBouncy(obj);
                }

                // 높이를 증감할 value를 시간에 따라 증가 (0 ~ 1)
                addValue += Time.deltaTime * bounceSpeed;

                // 아래로 이동하는 중에 플레이어가 개입할 경우 플레이어를 밀쳐내는 로직
                // 아래로 이동하는 중인가? 
                if (bouncyDir == -1)
                {
                    // 아래에 플레이어가 개입해있는가?
                    if (CheckExistPlayer(obj, Dir.down))
                    {
                        // 플레이어와 오브젝트간 방향을 구한 뒤에 그 방향으로 1만큼 떨어진 위치로 플레이어를 이동시킴 
                        Vector3 playerDir = (obj.transform.position - player.position).normalized;
                        playerDir.y = 0;
                        Vector3 targetPos = Vector3Int.RoundToInt(player.position - playerDir);

                        //player.GetComponent<Rigidbody>().MovePosition(targetPos);
                        while (Vector3.Distance(player.position, targetPos) > 0.3f)
                        {
                            player.position = Vector3.MoveTowards(player.position, targetPos, 0.2f);
                        }
                    }
                }

                //실제로 물체의 포지션을 변경하는 코드
                obj.transform.position = Vector3.Lerp(obj.transform.position, obj.transform.position + new Vector3(0, addValue * bouncyDir, 0), Time.deltaTime * bounciness);

            }
            // 계속 반복 (repeat Adj)
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator AbandonBouncy(InteractiveObject thisObject)
    {        
        isBouncy = false;

        // 즉시 rigidBody을 원상복귀
        if (!thisObject.CheckAdjective(EAdjective.Float))
        {
            var rb = thisObject.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        //Vector3 playerPos = new Vector3(thisObject.transform.position.x, thisObject.transform.position.y, thisObject.transform.position.z);
        //Vector3 prePos = Vector3Int.RoundToInt(playerPos);

        //var dict = DetectManager.GetInstance.GetArrayObjects(prePos + Vector3.up);
        //if (dict[prePos + Vector3.up] == thisObject.gameObject)
        //{
        //    DetectManager.GetInstance.SwapBlockInMap(prePos, prePos + Vector3.up);
        //}

        //while (!CheckExistBlock(thisObject.gameObject, Dir.down) && !CheckExistPlayer(thisObject.gameObject, Dir.down))
        //{
        //    if (prePos != Vector3Int.RoundToInt(thisObject.transform.position))
        //    {
        //        DetectManager.GetInstance.SwapBlockInMap(prePos, Vector3Int.RoundToInt(thisObject.transform.position));
        //    }
        //    yield return new WaitForEndOfFrame();
        //}

        yield return null;
    }
}