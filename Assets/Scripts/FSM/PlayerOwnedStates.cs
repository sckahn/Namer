using System.Collections;
using UnityEngine;

namespace PlayerOwnedStates
{
	public class IdleState : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
        {
            if (entity.pAnimator)
            {
                entity.pAnimator.SetBool("isRun", false);
            }
        }

		public void Execute(PlayerEntity entity)
		{
        }

        public void Exit(PlayerEntity entity)
		{
		}
	}

    public class WalkState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
        }

        public void Execute(PlayerEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Exit(PlayerEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }

	public class RunState : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
		{
            entity.pAnimator.SetBool("isRun", true);
        }

        public void Execute(PlayerEntity entity)
		{
        }

        public void Exit(PlayerEntity entity)
		{
            if (GameManager.GetInstance.isPlayerDoAction != true)
            {
                entity.pAnimator.SetBool("isRun", false);
            }
        }
    }
    public class ObtainState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isObtain", true);
            //GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.pAnimator.GetCurrentAnimatorStateInfo(0).IsName("Obtain") &&
                entity.pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }        
        }

        public void Exit(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isObtain", false);
        }
    }
    
    public class ClimbState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isClimb", true);
            //GameManager.GetInstance.isPlayerDoAction = true;        
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.pAnimator.GetCurrentAnimatorStateInfo(0).IsName("Climb") &&
                entity.pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                entity.ChangeState(PlayerStates.Idle);
            }                
        }

        public void Exit(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isClimb", false);
        }
    }
    
    public class PushState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isPush", true);
            GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.pAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push") &&
                entity.pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isPush", false);
        }
    }

    public class AddCardState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isAddCard", true);
            GameManager.GetInstance.isPlayerDoAction = true;
            InteractionSequencer.GetInstance.PlayerActionQueue.Enqueue(GameManager.GetInstance.localPlayerMovement.AddcardRootmotion());
            CardManager.GetInstance.ableCardCtr = false;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.pAnimator.GetCurrentAnimatorStateInfo(0).IsName("AddCard") &&
                entity.pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isAddCard", false);
            CardManager.GetInstance.ableCardCtr = true;
        }
    }

    public class WinState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isVictory", true);
            GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.pAnimator.GetCurrentAnimatorStateInfo(0).IsName("Victory") &&
                entity.pAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.pAnimator.SetBool("isVictory", false);
            GameManager.GetInstance.isPlayerDoAction = false;
        }
    }

}

