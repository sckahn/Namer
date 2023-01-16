using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

public enum PlayerStates { Idle = 0, Run, Push, EndPoint }

public class Player : BaseGameEntity
{	
	private	State<Player>[]	states;
	private	StateMachine<Player> stateMachine;
	public override void Setup(string name)
	{
		base.Setup(name);
		//EntityStatesInit();
		//states = new State<Player>[];
		states[(int)PlayerStates.Idle]	= new StudentOwnedStates.Idle();
		states[(int)PlayerStates.Run]	= new StudentOwnedStates.Run();

		stateMachine = new StateMachine<Player>();
		stateMachine.Setup(this, states[(int)PlayerStates.Idle]);
	}

	public void EntityStatesInit(string nameSpace)
	{
		// StateAlloc
		foreach (var e in Assembly.GetExecutingAssembly().GetTypes())
		{
			if (e.Namespace == nameSpace)
			{
			}
		}
    }

	public override void Updated()
	{
		stateMachine.Execute();
	}

	public void ChangeState(PlayerStates newState)
	{
		stateMachine.ChangeState(states[(int)newState]);
	}
}

