using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMaster : Singleton<InteractionMaster>
{
    public bool isPause = false;

    public int targetGUID;
    // 전반적인 인터렉션 관장
    
    // 플레이어 OR ADD Card로 인터렉션 시 다른 오브젝트 애니메이션 && 배열 함수 Sorting 정지
    // 포즈 조건은 얘가 관장하는 인터렉션일 경우에 포즈
    
    // --> 결국 한 함수안에 담겨야 하는거 아닌가..
    // 아니면 예외처리가 각 인터렉션마다 들어가야 한다.
    // --> 결국 while문 안에 while문이 또 들어가야함 (현재 마땅한 방법이 떠오르질 않음)
    
    
    void InteractionControl()
    {
        
    }
}
