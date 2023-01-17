using UnityEngine;
using System.Collections.Generic;
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
        //Player Move
        if (inputVec == Vector3.zero)
        {
            playerEntity.ChangeState(PlayerStates.Idle);
            return;
        }

        playerEntity.ChangeState(PlayerStates.Run);
        float gravity = rb.velocity.y;

        Vector3 velocity = moveSpeed * new Vector3(inputVec.x, 0, inputVec.z);
        velocity.y = gravity;
        rb.velocity = velocity;

        //Player Rotation
        if (velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotateSpeed);
        }
    }
        
    private Vector3 PlayerInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void PlayInteraction()
    {
        // TODO 인터렉션 시 해당 Obj 방향으로 정렬
        // Coroutine 사용?

        if (!playerEntity.doInteraction)
        {
            GameManager.GetInstance.GetCheckSurrounding.CheckCharacterCurrentTile(this.gameObject);
            GameManager.GetInstance.GetCheckSurrounding.CheckForwardObj(this.gameObject);

            if (Input.GetKeyDown(playerEntity.interactionKey) &&
                GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo)
            {
                interactobj = GameManager.GetInstance.GetCheckSurrounding.forwardObjectInfo;

                if (interactobj.tag == "InteractObj")
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

        // 인터렉션 중에는 이동 또는 다른 인터렉션 불가
        if (_canPlayerInput && !playerEntity.doInteraction)
        {
            // 이동 함수 + 인터렉션
            PlayerMove(PlayerInput());
            PlayInteraction();
        }
    }

    public void PlayPushAnimation1()
    {
        StartCoroutine(AnimationPlay());    
    }

    Vector3 CalculateTargetPos()
    {

        Vector3 targetPos = new Vector3();

        if ((int)GameManager.GetInstance.GetCheckSurrounding.mydir == 0)
        {
            targetPos += Vector3.forward;
        }

        else if ((int)GameManager.GetInstance.GetCheckSurrounding.mydir == 3)
        {
            targetPos += Vector3.right;
        }

        else if ((int)GameManager.GetInstance.GetCheckSurrounding.mydir == 1)
        {
            targetPos += Vector3.back;
        }

        else if ((int)GameManager.GetInstance.GetCheckSurrounding.mydir == 2)
        {
            targetPos += Vector3.left;
        }

        return targetPos;
    }

    public IEnumerator AnimationPlay()
    {
        Vector3 targetPos = CalculateTargetPos();
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

