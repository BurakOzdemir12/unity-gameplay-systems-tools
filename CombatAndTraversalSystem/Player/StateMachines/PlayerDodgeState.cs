using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerDodgeState : PlayerBaseState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string DODGE_TAG = "Dodge";
        private float previousFrameTime;
        private Vector3 direction;


        public override void Enter()
        {
            //TODO Dodge will be different, In Alert mode and in Safety mode

            //TODO Get attack direction of enemy and,  dodge accordingly 
            
            // stateMachine.Animator.applyRootMotion = true;
            direction = CalculateMovementDirection();
            if (direction.sqrMagnitude < 0.0001f)
            {
                stateMachine.Animator.CrossFadeInFixedTime(stateMachine.DodgeBackwardHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.DodgeForwardHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            float normalisedTime = GetNormalizedTime(stateMachine.Animator, 0, DODGE_TAG);

            if (normalisedTime >= 1f)
            {
                // stateMachine.Animator.applyRootMotion = false;
                stateMachine.DecideTargetOrLocomotion();
                return;
            }


            // Disable Move Functşon call if you going to use root Motion
            Vector3 dodgeDirection = direction.normalized;
            Vector3 movement = dodgeDirection * stateMachine.DodgeSpeed;
            if (direction.sqrMagnitude < 0.0001f)
            {
                Vector3 backward = -stateMachine.MainCameraTransform.forward;
                backward.y = 0f;
                backward.Normalize();

                if (normalisedTime >= stateMachine.DodgeAnimStartTime &&
                    normalisedTime <= stateMachine.DodgeAnimEndTime)
                {
                    Move(backward * stateMachine.DodgeSpeed, deltaTime);
                }
            }
            else
            {
                Move(movement, deltaTime);
            }
        }

        public override void Exit()
        {
            // stateMachine.Animator.applyRootMotion = false;
        }
    }
}