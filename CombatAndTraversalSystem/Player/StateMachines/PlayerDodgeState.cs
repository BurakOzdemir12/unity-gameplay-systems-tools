using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerDodgeState : PlayerBaseState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string DODGE_TAG = "Dodge";
        private Vector3 direction;
        private float normalizedTime;
        private bool isFromTargetingState;

        public override void Enter()
        {
            //TODO Dodge will be different, In Alert mode and in Safety mode

            //TODO Get attack direction of enemy and,  dodge accordingly

            Vector2 movement = stateMachine.InputHandler.Move;
            isFromTargetingState = stateMachine.PreviousState is PlayerTargetingState;
            if (isFromTargetingState)
            {
                stateMachine.Animator.applyRootMotion = true;

                int dodgeHash = PickTargetingDodge(movement);

                stateMachine.Animator.CrossFadeInFixedTime(dodgeHash, stateMachine.CrossFadeDuration);
                return;
            }

            // if came from Free Look state then it moves without root motion
            // Why I did this? I don't know its 4 in the morning


            stateMachine.Animator.applyRootMotion = false;
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
            normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, DODGE_TAG);

            if (normalizedTime >= 1f)
            {
                // stateMachine.Animator.applyRootMotion = false;
                stateMachine.DecideTargetOrLocomotion();
                return;
            }


            // Disable Move Functşon call if you going to use root Motion
            if (!isFromTargetingState)
            {
                Vector3 dodgeDirection = direction.normalized;
                Vector3 movement = dodgeDirection * stateMachine.DodgeSpeed;

                bool isAnimationActive = AnimationCalculator();
                if (!isAnimationActive) return;
                float rollDampTime = stateMachine.RotationDampTimeWhileRoll;

                RotateFaceToLook(deltaTime, rollDampTime);
                if (direction.sqrMagnitude < 0.0001f)
                {
                    Vector3 backward = -stateMachine.MainCameraTransform.forward;
                    backward.y = 0f;
                    backward.Normalize();

                    Move(backward * stateMachine.DodgeSpeed, deltaTime);
                }
                else
                {
                    Move(movement, deltaTime);
                }
            }
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
        }

        private bool AnimationCalculator()
        {
            return normalizedTime >= stateMachine.DodgeAnimStartTime &&
                   normalizedTime <= stateMachine.DodgeAnimEndTime;
        }

        private int PickTargetingDodge(Vector2 movement)
        {
            // float forwardSpeed = stateMachine.Animator.GetFloat(stateMachine.TargetingForwardSpeedHash);
            // float rightSpeed = stateMachine.Animator.GetFloat(stateMachine.TargetingRightSpeedHash);

            if (movement.sqrMagnitude < 0.0001f)
            {
                return stateMachine.DodgeBackwardHash;
            }

            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                return movement.x > 0f ? stateMachine.DodgeRightHash : stateMachine.DodgeLeftHash;

            return movement.y > 0f ? stateMachine.DodgeForwardHash : stateMachine.DodgeBackwardHash;
        }
    }
}