using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSequencer : Singleton<InteractionSequencer>
{
    public Queue<IEnumerator> CoroutineQueue;
    public Queue<IEnumerator> InteractionQueue; // PlayerInteraction OR AddCard 
    private bool isPlayerInteractionPlaying = false;
    
    // 전반적인 인터렉션 관장
    // 플레이어 OR ADD Card로 인터렉션 시 다른 오브젝트 애니메이션 && 배열 함수 Sorting 정지 --> IEnumerator 제어로 결정
    
    private void Start()
    {
        CoroutineQueue = new Queue<IEnumerator>();
        InteractionQueue = new Queue<IEnumerator>();
        StartCoroutine(SequentialCoroutine());
    }

    // TODO DemoDay 이후에 Exception 정의하기....ㅠ
    #region AdjExceptions & SortingLayers
    
    // Sorting Layer 만들기
    // Ex 1 .. 진자운동 + Fire 일 경우 Coroutine이 병렬 시행되어야 함..
    // Ex 2 .. Fire + 주변 오브젝트 밀기 일 경우 Coroutine이 순차 시행되어야 함..

    
    // Adj Exceptions 만들기
    
    #endregion

    #region SequenceFunction TestCode
    private int InteractionCounter = 0;
    [SerializeField] private KeyCode TestInteractionKey = KeyCode.H;
    [SerializeField] private KeyCode SubRoutineStartKey = KeyCode.G;
    private bool isSubRoutineUpdate = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(TestInteractionKey))
        {
            InteractionQueue.Enqueue(PriorityCoroutine());
        }

        if (Input.GetKeyDown(SubRoutineStartKey))
        {
            isSubRoutineUpdate = !isSubRoutineUpdate;
        }
    }
    
    private void FixedUpdate()
    {
        if (isSubRoutineUpdate)
        {
            CoroutineQueue.Enqueue(subRoutineTest());
        }
    }
    
    IEnumerator subRoutineTest()
    {
        Debug.Log("Some Object are Interacting.....");
        
        yield return new WaitForSeconds(.1f);
    }

    IEnumerator PriorityCoroutine()
    {
        Debug.Log("Interation is Playing......");
        
        yield return new WaitForSeconds(5f);

        Debug.Log("Interaction is End : Interatction Counter  : " + ++InteractionCounter);
    }

    #endregion

    public IEnumerator WaitUntilPlayerInteractionEnd()
    {
        yield return new WaitUntil(() => isPlayerInteractionPlaying == false);
        //수정한 부분
        DetectManager.GetInstance.StartDetector();
        //수정한 부분 
    }


    // PlayerInteraction OR Addcard로 인한 Coroutine 제어
    private IEnumerator SequentialCoroutine()
    {
        while (true)
        {
            // 인터렉션이 먼저 실행되어야 하는 경우
            if (InteractionQueue.Count > 0)
            {
                isPlayerInteractionPlaying = true;
                
                // ConcurrentCoroutines
                // 모든 코루틴 다 꺼내기 (어떤 코루틴이 먼저 실행 될지 몰라도 되는 경우)
                while (CoroutineQueue.Count > 0)
                {
                    StartCoroutine(CoroutineQueue.Dequeue());
                }

                yield return StartCoroutine(InteractionQueue.Dequeue());
                isPlayerInteractionPlaying = false;
            }

            else
            {
                while (CoroutineQueue.Count > 0)
                {
                    StartCoroutine(CoroutineQueue.Dequeue());
                }
            }

            yield return null;
        }
    }
}
