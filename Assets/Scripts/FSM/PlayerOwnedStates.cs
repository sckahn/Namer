using Unity.VisualScripting;
using UnityEngine;

namespace PlayerOwnedStates
{
	public class IdleState : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
		{
        }

		public void Execute(PlayerEntity entity)
		{
            GameManager.GetInstance.isPlayerDoInteraction = false;
        }

        public void Exit(PlayerEntity entity)
		{
		}
	}

	public class RunState : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
		{
			entity.myAnimator.SetBool("isRun", true);
        }

        public void Execute(PlayerEntity entity)
		{
            GameManager.GetInstance.isPlayerDoInteraction = false;
        }

        public void Exit(PlayerEntity entity)
		{
            entity.myAnimator.SetBool("isRun", false);
        }
    }

    public class PushState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", true);
            GameManager.GetInstance.isPlayerDoInteraction = true;
            
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.RevertToPreviousState();
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", false);
            GameManager.GetInstance.isPlayerDoInteraction = false;
        }
    }

    public class WinState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isVictory", true);
            GameManager.GetInstance.isPlayerDoInteraction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Victory") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isVictory", false);
            GameManager.GetInstance.isPlayerDoInteraction = false;
        }
    }

}

