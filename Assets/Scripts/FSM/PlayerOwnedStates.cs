using System.Collections;
using UnityEngine;

namespace PlayerOwnedStates
{
	public class IdleState : IState<PlayerEntity>
	{
		public void Enter(PlayerEntity entity)
        {
            if (entity.myAnimator)
            {
                entity.myAnimator.SetBool("isRun", false);
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
            entity.myAnimator.SetBool("isRun", true);
        }

        public void Execute(PlayerEntity entity)
		{
        }

        public void Exit(PlayerEntity entity)
		{
            if (GameManager.GetInstance.isPlayerDoAction != true)
            {
                entity.myAnimator.SetBool("isRun", false);
            }
        }
    }
    public class ObtainState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isObtain", true);
            //GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Obtain") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }        
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isObtain", false);
        }
    }
    
    public class ClimbState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isClimb", true);
            //GameManager.GetInstance.isPlayerDoAction = true;        
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Climb") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                entity.ChangeState(PlayerStates.Idle);
            }                
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isClimb", false);
        }
    }
    
    public class PushState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", true);
            GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Push") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", false);
        }
    }

    public class AddCardState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isAddCard", true);
            GameManager.GetInstance.isPlayerDoAction = true;
            InteractionSequencer.GetInstance.PlayerActionQueue.Enqueue(GameManager.GetInstance.localPlayerMovement.AddcardRootmotion());
            CardManager.GetInstance.ableCardCtr = false;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("AddCard") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.ChangeState(PlayerStates.Idle);
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isAddCard", false);
            CardManager.GetInstance.ableCardCtr = true;
        }
    }

    public class WinState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isVictory", true);
            GameManager.GetInstance.isPlayerDoAction = true;
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
            GameManager.GetInstance.isPlayerDoAction = false;
        }
    }

}

