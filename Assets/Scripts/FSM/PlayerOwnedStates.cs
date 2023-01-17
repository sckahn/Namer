using UnityEngine;

namespace PlayerOwnedStates
{
	public class Idle : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
		{
        }

		public void Execute(PlayerEntity entity)
		{
            entity.doInteraction = false;
        }

        public void Exit(PlayerEntity entity)
		{
		}
	}

	public class Run : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
		{
			entity.myAnimator.SetBool("isRun", true);
        }

        public void Execute(PlayerEntity entity)
		{
		}

		public void Exit(PlayerEntity entity)
		{
            entity.myAnimator.SetBool("isRun", false);
        }
    }

    public class Push : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", true);
            entity.doInteraction = true;
        }

        public void Execute(PlayerEntity entity)
        {
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", false);
            entity.doInteraction = false;
        }
    }

}

