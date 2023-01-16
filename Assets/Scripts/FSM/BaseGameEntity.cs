using UnityEngine;

public abstract class BaseGameEntity : MonoBehaviour
{
	private static int m_iNextValidID = 0;
	private	int	id;

	public	int	ID
	{
		set
		{
			id = value;
			m_iNextValidID ++;
		}
		get => id;
	}

	private	string entityName;

	public virtual void Setup(string name)
	{
		ID = m_iNextValidID;
		entityName = name;
	}

	public abstract void Updated();
}

