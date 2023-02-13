using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FunctionComparer : IComparer<IEnumerator>
{
    enum EnumeratorFunctionName
    {
        MoveObj,
        SetGrowScale,
        BounceObj
    }

    public int Compare(IEnumerator functionA, IEnumerator functionB)
    {
        // Extract Function Name
        string[] fNames = { functionA?.GetType().Name, functionB?.GetType().Name };
        EnumeratorFunctionName[] fNameOrders = new EnumeratorFunctionName[2];

        for (int i = 0; i < fNames.Length; ++i)
        {
            if (fNames[i] != null)
            {
                fNames[i] = fNames[i].Substring(fNames[i].IndexOf('<') + 1, fNames[i].IndexOf('>') - 1);
                if (Enum.IsDefined(typeof(EnumeratorFunctionName), fNames[i]))
                {
                    fNameOrders[i] = Enum.Parse<EnumeratorFunctionName>(fNames[i]);
                }

                else
                {
                    Debug.LogError("함수 이름이 Enum에 등록되지 않은 함수입니다!!");
                }
            }

            else
            {
                Debug.LogError("함수 이름이 Null 입니다!!");
            }
        }

        if (fNameOrders[0] > fNameOrders[1])
        {
            return 1;
        }
        
        else if (fNameOrders[0] < fNameOrders[1])
        {
            return -1;
        }

        else
        {
            return 0;
        }
    }
}

public class InteractionSequencer : Singleton<InteractionSequencer>
{
    public Queue<IEnumerator> CoroutineQueue;                   // One-Off Coroutine Queue
    public Queue<IEnumerator> SequentialQueue;                  // Obj <--> Obj Interaction Coroutine Sequence Queue
    public Queue<IEnumerator> PlayerActionQueue;                // PlayerInteraction OR AddCard Coroutine Queue
    public InteractiveObject playerActionTargetObject;
    //public GameObject player;
    // 전반적인 인터렉션 관장
    // 플레이어 OR ADD Card로 인터렉션 시 다른 오브젝트 애니메이션 && 배열 함수 Sorting 정지 --> IEnumerator 제어로 결정
    
    private void Start()
    {
        CoroutineQueue = new Queue<IEnumerator>();
        SequentialQueue = new Queue<IEnumerator>();
        PlayerActionQueue = new Queue<IEnumerator>();
        StartCoroutine(SequentialCoroutine());
    }

    public IEnumerator WaitUntilPlayerInteractionEnd(IAdjective currentInteractiveAdjectiveObject)
    {
        // 변수 2개 필요
        // 플레이어가 인터렉션 하고 있는 오브젝트 --> 코루틴 정지하지 않음
        // 플레이어가 인터렉션 하고 있지 않는 오브젝트 --> 코루틴 정지 필요
        
        if (currentInteractiveAdjectiveObject == null || playerActionTargetObject)
        {
            yield break;
        }
        
        // 플레이어가 인터렉션 하고 있는 객체이면 Break
        if (playerActionTargetObject.Adjectives.Contains(currentInteractiveAdjectiveObject))
        {
            yield break;
        }
        
        yield return new WaitUntil(() => GameManager.GetInstance.isPlayerDoAction == false);
        //수정한 부분
        // DetectManager.GetInstance.StartDetector();
        //수정한 부분 
    }
    
    // PlayerInteraction OR Addcard로 인한 Coroutine 제어
    private IEnumerator SequentialCoroutine()
    {
        while (true)
        {
            // Player Action이 진행되면 다른 Coroutine을 잠시 멈추게 한다. (PlayerActionQueue이외의 다른 Queue를 Dequeue 하지 않음)
            if (PlayerActionQueue.Count > 0)
            {
                GameManager.GetInstance.isPlayerDoAction = true;
                
                
                yield return StartCoroutine(PlayerActionQueue.Dequeue());
                
                // ConcurrentCoroutines
                // 어떤 코루틴이 먼저 실행 될지 몰라도 되는 코루틴들 (1, 2 코루틴들)
                // 1. One-Off 코루틴 다 꺼내기
                while (CoroutineQueue.Count > 0)
                {
                    // (input-parameters) => { <sequence-of-statements> }
                    StartCoroutine(CoroutineQueue.Dequeue());
                    yield return null;
                }
                
                // 2. 순차 코루틴 꺼내기 
                while (SequentialQueue.Count > 0)
                {
                    if (SequentialQueue.Count > 1)
                    {
                        // Queue Sorting
                        Queue<IEnumerator> sortedSequentialQueue =
                            new Queue<IEnumerator>(SequentialQueue.OrderBy(x => x, new FunctionComparer()));
                        yield return StartCoroutine(sortedSequentialQueue.Dequeue());
                    }
                    
                    yield return StartCoroutine(SequentialQueue.Dequeue());
                }
            }

            else
            {
                while (CoroutineQueue.Count > 0)
                {
                    StartCoroutine(CoroutineQueue.Dequeue());
                    yield return null;
                }
                
                while (SequentialQueue.Count > 0)
                {
                    if (SequentialQueue.Count > 1)
                    {
                        Queue<IEnumerator> sortedSequentialQueue =
                            new Queue<IEnumerator>(SequentialQueue.OrderBy(x => x, new FunctionComparer()));
                        yield return StartCoroutine(sortedSequentialQueue.Dequeue());
                    }
                    
                    yield return StartCoroutine(SequentialQueue.Dequeue());
                }
            }

            yield return null;
        }
    }
}
