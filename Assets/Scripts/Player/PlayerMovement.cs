using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    #region components
    private Rigidbody rb;
    public PlayerEntity playerEntity;
    #endregion

    public GameObject interactObj;
    public GameObject addCardTarget;
    public float moveSpeed;
    public int rotateSpeed;
    public Vector3 inputVector;
    private Dir targetDir;
    private int objscale;

    [SerializeField] [Range(0.1f, 5f)] private float rootmotionSpeed;
    [SerializeField] [Range(0.5f, 5f)] private float interactionDelay;
    // TODO KeyMapping ?

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerEntity = GetComponent<PlayerEntity>();
        GameManager.GetInstance.KeyAction += MoveKeyInput;
        GameManager.GetInstance.localPlayerMovement = this;
        
        #region Init Variable
        rootmotionSpeed = 1f;
        interactionDelay = 2f;
        moveSpeed = 3f;
        rotateSpeed = 10;
        #endregion
    }

    private void PlayerMove(Vector3 inputVec)
    {
        float gravity = rb.velocity.y;

        //Player Move
        if (inputVec == Vector3.zero)
        {
            rb.velocity = new Vector3(inputVec.x, gravity, inputVec.z);
            playerEntity.ChangeState(PlayerStates.Idle);
            return;
        }

        Vector3 velocity = moveSpeed * new Vector3(inputVec.x, 0, inputVec.z);
        velocity.y = gravity;
        rb.velocity = velocity;
        playerEntity.ChangeState(PlayerStates.Run);

        //Player Rotation
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotateSpeed);
        }
    }
    
    public void MoveKeyInput()
    {
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void PlayInteraction()
    {
        if (Input.GetKeyDown(GameManager.GetInstance.interactionKey))
        {
            if (interactObj && interactObj.CompareTag("InteractObj"))
            {
                targetDir = DetectManager.GetInstance.objDir;
                InteractionSequencer.GetInstance.playerActionTargetObject = interactObj.GetComponent<InteractiveObject>();
                if (InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[1] != null)
                {
                    InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[1].Execute(
                        InteractionSequencer.GetInstance.playerActionTargetObject, this.gameObject);
                    return;
                }
                
                if (InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[2] != null)
                {
                    InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[2].Execute(
                        InteractionSequencer.GetInstance.playerActionTargetObject, this.gameObject);
                    return;
                }
                
                if (InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[6] != null)
                {
                    InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[6].Execute(
                        InteractionSequencer.GetInstance.playerActionTargetObject, this.gameObject);
                    return;
                }
                
                if (InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[7] != null)
                {
                    objscale = (int)transform.position.y - (int)InteractionSequencer.GetInstance.playerActionTargetObject.transform.position.y +
                        (int)InteractionSequencer.GetInstance.playerActionTargetObject.transform.localScale.y;
                    InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives[7].Execute(
                        InteractionSequencer.GetInstance.playerActionTargetObject, this.gameObject);
                    return;
                }
            }
        }
    }

    private void Update()
    {
        DetectManager.GetInstance.CheckCharacterCurrentTile(this.gameObject);
        DetectManager.GetInstance.CheckForwardObj(this.gameObject);
        interactObj = DetectManager.GetInstance.forwardObjectInfo;

        // 인터렉션 중에는 이동 또는 다른 인터렉션 불가
        // 플레이어 인풋이 막힌 경우 동작하지 않도록 변경
        if (GameManager.GetInstance.isPlayerCanInput && !GameManager.GetInstance.isPlayerDoAction)
        {
            // 이동 함수 + 인터렉션
            PlayerMove(inputVector);
            PlayInteraction();
        }
    }

    // 방향만 맞춰주면 되는 경우
    public IEnumerator SetRotationBeforeInteraction()
    {
        float targetRotationY = 0;
        //float curPlayerRoationY = transform.eulerAngles.y;
        switch (targetDir)
        {
            case Dir.right :
                targetRotationY = 90;
                break;
            case Dir.down :
                targetRotationY = 180;
                break;
            case Dir.left :
                targetRotationY = 270;
                break;
            case Dir.up :
                targetRotationY = 0;
                break;
            default :
                Debug.LogError("잘못된 타겟 방향값입니다...!");
                break;
        }

        // if (this.transform.rotation.y < 0)
        // {
        //     curPlayerRoationY += 360;
        // }
        //float destinationRotationY = targetRotationY - curPlayerRoationY;
        
        // TODO Lerp Lotation 하기
        //float rotationTime = 0;
        transform.rotation = Quaternion.Euler(new Vector3(0, targetRotationY, 0));

        // while (rotationTime < 1 )
        // {
        //     rotationTime += Time.deltaTime;
        //     if (rotationTime > 1f)
        //     {
        //         rotationTime = 1;
        //     }
        //     
        //     yield return new WaitForEndOfFrame();
        // }
        
        yield return null;
    }
    
    
    // 포지션도 맞춰줘야 할 경우
    public IEnumerator SetPositionBeforeInteraction(Transform targetObjTransform)
    {
        // TODO 포지션 맞춰줘야 하는 경우 고민
        
        yield return null;
    }

    #region Animation Rootmotion
    public IEnumerator PushRootmotion()
    {
        yield return SetRotationBeforeInteraction();

        Vector3 targetPos = Vector3.zero;
        
        switch (targetDir)
        {
            case Dir.right :
                targetPos = Vector3.right;
                break;
            case Dir.down :
                targetPos = Vector3.back;
                break;
            case Dir.left :
                targetPos = Vector3.left;
                break;
            case Dir.up :
                targetPos = Vector3.forward;
                break;
            default :
                Debug.LogError("잘못된 타겟 방향값입니다...!");
                break;
        }
        var curPos = transform.position;
        Vector3 destinationPos = targetPos + curPos;
        float moveTime = 0;
        
        while (moveTime < 1)
        {
            moveTime += Time.deltaTime * rootmotionSpeed; 
            transform.position = Vector3.Lerp(curPos, destinationPos, moveTime + 0.1f);
            yield return null;
        }

        yield return new WaitForSeconds(interactionDelay);
        GameManager.GetInstance.isPlayerDoAction = false;
    }

    public IEnumerator ClimbRootmotion()
    {
        yield return SetRotationBeforeInteraction();
        var position = transform.position;
        position = new Vector3((float)Math.Round(position.x, 1),
            (float)Math.Round(position.y, 1), (float)Math.Round(position.z, 1));
        transform.position = position;

        Vector3 targetPos = Vector3.zero;
        var curPos = position;
        
        switch (targetDir)
        {
            case Dir.right :
                targetPos = Vector3.right;
                break;
            case Dir.down :
                targetPos = Vector3.back;
                break;
            case Dir.left :
                targetPos = Vector3.left;
                break;
            case Dir.up :
                targetPos = Vector3.forward;
                break;
            default :
                Debug.LogError("잘못된 타겟 방향값입니다...!");
                break;
        }

        float moveTime = 0;
        Vector3 target1 = curPos + Vector3.up * (0.5f * objscale);
        yield return new WaitForSeconds(1f);

        while (moveTime < 1)
        {
            moveTime += Time.deltaTime * rootmotionSpeed;
            if (transform.position.y > target1.y)
            {
                yield return null;
            }
            else
            {
                transform.position = Vector3.Lerp(curPos, target1, moveTime);
            }
            yield return null;
        }

        transform.position = target1;
        moveTime = 0;
        curPos = transform.position;
        
        while (moveTime < 1)
        {
            moveTime += Time.deltaTime * rootmotionSpeed; 
            transform.position = Vector3.Lerp(curPos, curPos + targetPos * 0.5f, moveTime);
            yield return null;
        }

        curPos = transform.position;
        moveTime = 0;
        Vector3 target2 = curPos + (Vector3.up * (objscale * 0.5f));
        while (moveTime < 1f)
        {
            moveTime += Time.deltaTime * rootmotionSpeed; 
            if (transform.position.y > target2.y)
            {
                yield return null;
            }
            else
            {
                transform.position = Vector3.Lerp(curPos, target2, moveTime);
            }
            yield return null;
        }

        //transform.position = target2;

        yield return new WaitForSeconds(interactionDelay);
        GameManager.GetInstance.isPlayerDoAction = false;
    }

    public IEnumerator AddcardRootmotion()
    {
        transform.rotation = Quaternion.LookRotation(new Vector3(addCardTarget.transform.position.x, 0f, addCardTarget.transform.position.z));

        yield return new WaitForSeconds(interactionDelay);
        
        yield return null;
        GameManager.GetInstance.isPlayerDoAction = false;
    }
    #endregion

    #region AnimationEvent Function
    public void PushRootmotionEvent()
    {
        StartCoroutine(PushRootmotion());    
    }

    public void ClimbRootmotionEvent()
    {
        StartCoroutine(ClimbRootmotion());
    }

    public void AddCardRootmotionEvent()
    {
        StartCoroutine(AddcardRootmotion());
    }
    #endregion

}


