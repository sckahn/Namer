using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region components
    private Rigidbody rb;
    private Animator myanimator;
    private PlayerEntity playerEntity;
    #endregion

    public GameObject interactobj;
    public float moveSpeed = 3;
    public int rotateSpeed = 10;
    public bool _canPlayerInput = true;

    // TODO KeyMapping ?

    private void Start()
    {
        myanimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerEntity = GetComponent<PlayerEntity>();
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
        
    private Vector3 PlayerInput()
    {
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void PlayInteraction()
    {
        if (Input.GetKeyDown(playerEntity.interactionKey))
        {
            // TODO 인터렉션 시 해당 Obj 방향으로 정렬
            
            if (GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo)
            {
                interactobj = GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo;

                if (interactobj.CompareTag("InteractObj"))
                {
                    playerEntity.ChangeState(PlayerStates.Push);
                    interactobj.GetComponent<InteractiveObject>().Interact(this.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        // TODO GameManager한테 인풋 받을 수 있는 변수 가져오기
        GameManager.GetInstance.GetCheckSurrounding.CheckCharacterCurrentTile(this.gameObject);
        GameManager.GetInstance.GetCheckSurrounding.CheckForwardObj(this.gameObject);
        interactobj = GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo;



        Debug.Log((int)(this.transform.rotation.eulerAngles.y % 90) + " : 나머지  ");
        
        if (interactobj)
        {
            var position = interactobj.transform.position;
            var position1 = this.transform.position;
            //Vector3 fwd = this.transform.TransformDirection(Vector3.forward);
            //Debug.Log(Mathf.Rad2Deg * Mathf.Atan2(fwd.x, fwd.z));
            //Debug.Log(Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(new Vector2(position.x, position.z), new Vector2(fwd.x, fwd.z)) /
            //new Vector2(position.x, position.z).magnitude * new Vector2(fwd.x, fwd.z).magnitude));
        }

        // 인터렉션 중에는 이동 또는 다른 인터렉션 불가
        if (_canPlayerInput && !GameManager.GetInstance.isPlayerDoInteraction)
        {
            // 이동 함수 + 인터렉션
            PlayerMove(PlayerInput());
            PlayInteraction();
        }
    }

    public void PushRootmotionEvent()
    {
        StartCoroutine(PushRootmotion(interactobj.transform));    
    }


    private void Dircheck(GameObject targetobj)
    {
        if (targetobj)
        {
            
        }
    }

    public IEnumerator test()
    {
        
        GameManager.GetInstance.GetCheckSurrounding.CheckCharacterCurrentTile(this.gameObject);
        GameManager.GetInstance.GetCheckSurrounding.CheckForwardObj(this.gameObject);
        interactobj = GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo;
        var position = interactobj.transform.position;
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.right)
        {
            //while (transform.rotation.y)
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.left)
        {
            
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.up)
        {
            
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.down)
        {
            
        }


        yield return null;

    }
    // 방향만 맞춰주면 되는 경우
    public IEnumerator SetRotationBeforeInteraction(Transform targetObjTransform)
    {
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.right)
        {
            //this.transform.rotation = Quaternion.Lerp();
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.left)
        {
            
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.up)
        {
            
        }
        
        if (GameManager.GetInstance.GetCheckSurrounding.objDir == Dir.down)
        {
            
        }
        
        while (true)
        {
            yield return new WaitForFixedUpdate();
        }
        
        
        yield return null;
    }
    
    
    // 포지션도 맞춰줘야 할 경우
    public IEnumerator SetPositionBeforeInteraction(Transform targetObjTransform)
    {
        var position = GameManager.GetInstance.GetCheckSurrounding.curObjectInfo.transform.position;
        Vector2 playerTilePos = new Vector2(position.x, position.z);
        
        Vector2 targetPos = new Vector2(targetObjTransform.position.x - playerTilePos.x,
            targetObjTransform.position.z - playerTilePos.y);
        float padding = 0.05f;
        
        // TODO Walk 애니메이션 사용하는 것도 좋을 듯.
        // while ()
        // {
        //     this.transform.position = 
        // }
        //
        // this.transform.position
        yield return null;
    }

    public IEnumerator PushRootmotion(Transform targetObjTransform)
    {
        yield return SetPositionBeforeInteraction(GameManager.GetInstance.GetCheckSurrounding.curObjectInfo.transform);
        
        Vector3 targetPos = targetObjTransform.position;
        float padding = 0.005f;
        Vector3 temp = targetPos + interactobj.transform.position;

        while ((interactobj.transform.position - temp).magnitude > 0.01f)
        {
            interactobj.transform.position = new Vector3(targetPos.x * padding + interactobj.transform.position.x,
                interactobj.transform.position.y,
                targetPos.z * padding + interactobj.transform.position.z);

            this.transform.position = new Vector3(targetPos.x * padding + this.transform.position.x,
             this.transform.position.y,
             targetPos.z * padding + this.transform.position.z);

            yield return new WaitForSeconds(0.01f);
        }

        //TODO 포지션 맞춰주기
        interactobj.transform.position = new Vector3(Mathf.Round(interactobj.transform.position.x), Mathf.Round(interactobj.transform.position.y), Mathf.Round(interactobj.transform.position.z));
    }
}

