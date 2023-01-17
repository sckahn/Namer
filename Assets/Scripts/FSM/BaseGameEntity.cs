using UnityEngine;

public abstract class BaseGameEntity : MonoBehaviour
{
    public virtual void Start()
    {
        Setup();
    }

    public virtual void Setup()
	{
        // TODO 리플렉션을 이용하여 클래스 동적 할당 생각해보기
    }

    public abstract void Updated();

}

