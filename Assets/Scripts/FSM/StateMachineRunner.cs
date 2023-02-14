using System.Collections.Generic;
using UnityEngine;

public class StateMachineRunner : MonoBehaviour
{	
	public	List<BaseGameEntity> entitys;

	private void Start()
	{
		entitys = new List<BaseGameEntity>();

		foreach (var e in GameObject.FindObjectsOfType<BaseGameEntity>())
		{
			entitys.Add(e);
		}
	}

	private void Update()
	{
		// TODO entity Update를 언제 종료할 것인가 고민 (종료조건)
		if (entitys.Count == 0)
		{
			return;
		}

		for (int i = 0; i < entitys.Count; ++i)
		{
			if (entitys[i])
			{
				entitys[i].Updated();
			}
		}
	}
}

