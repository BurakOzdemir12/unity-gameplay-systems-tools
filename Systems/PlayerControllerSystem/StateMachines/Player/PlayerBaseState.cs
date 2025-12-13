using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public abstract class PlayerBaseState : State
    {
        protected PlayerStateMachine stateMachine;

        public PlayerBaseState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void Move(Vector3 movement, float deltaTime)
        {
            var motion = stateMachine.ForceReceiver.Movement;
            stateMachine.Controller.Move((motion + movement) * deltaTime);
        }

        protected void FaceTarget(Target currentTarget, float deltaTime)
        {
            if (currentTarget == null) return;
            Vector3 targetLookPos = currentTarget.transform.position - stateMachine.transform.position;
            targetLookPos.y = 0; //if the player above or below the target we dont care..
            Quaternion targetRotation = stateMachine.transform.rotation = Quaternion.LookRotation(targetLookPos);
            Quaternion.Slerp(stateMachine.transform.rotation, targetRotation, deltaTime);
        }
    }
}