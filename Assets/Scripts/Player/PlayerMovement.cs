using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    #region components
    private Rigidbody rb;
    public PlayerEntity playerEntity;
    #endregion

    public GameObject interactobj;
    public float moveSpeed;
    public int rotateSpeed;
    public Vector3 inputVector;
    private Dir targetDir;

    [SerializeField] [Range(0.1f, 5f)] private float rootmotionSpeed;
    [SerializeField] [Range(0.5f, 5f)] private float interactionDelay;
    // TODO KeyMapping ?

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerEntity = GetComponent<PlayerEntity>();
        GameManager.GetInstance.KeyAction += MoveKeyInput;

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
            if (interactobj && interactobj.CompareTag("InteractObj"))
            {
                targetDir = DetectManager.GetInstance.objDir;
                InteractionSequencer.GetInstance.playerActionTargetObject = interactobj.GetComponent<InteractiveObject>();

                foreach (var e in InteractionSequencer.GetInstance.playerActionTargetObject.Adjectives)
                {
                    if (e != null)
                    {
                        GameManager.GetInstance.isPlayerDoAction = true;
                        e.Execute(InteractionSequencer.GetInstance.playerActionTargetObject, this.gameObject);
                    }
                }
            }
        }
    }

    private void Update()
    {
        
        
        DetectManager.GetInstance.CheckCharacterCurrentTile(this.gameObject);
        DetectManager.GetInstance.CheckForwardObj(this.gameObject);
        interactobj = DetectManager.GetInstance.forwardObjectInfo;

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

    #region AnimationEvent Function
    public void PushRootmotionEvent()
    {
        StartCoroutine(PushRootmotion());    
    }
    #endregion

}

