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
            GameManager.GetInstance.isPlayerDoAction = false;
        }

        public void Exit(PlayerEntity entity)
		{
		}
	}

    public class Walk : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            throw new System.NotImplementedException();
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
            GameManager.GetInstance.isPlayerDoAction = false;
        }

        public void Exit(PlayerEntity entity)
		{
            entity.myAnimator.SetBool("isRun", false);
        }
    }
    public class ObtainState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isObtain", true);
            GameManager.GetInstance.isPlayerDoAction = true;
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Obtain") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.RevertToPreviousState();
            }        
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isObtain", false);
            GameManager.GetInstance.isPlayerDoAction = false;        
        }
    }
    
    public class ClimbState : IState<PlayerEntity>
    {
        public void Enter(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isClimb", true);
            GameManager.GetInstance.isPlayerDoAction = true;        
        }

        public void Execute(PlayerEntity entity)
        {
            if (entity.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Climb") &&
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.RevertToPreviousState();
            }                
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isClimb", false);
            GameManager.GetInstance.isPlayerDoAction = false;        
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
                entity.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                entity.RevertToPreviousState();
            }
        }

        public void Exit(PlayerEntity entity)
        {
            entity.myAnimator.SetBool("isPush", false);
            GameManager.GetInstance.isPlayerDoAction = false;
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

