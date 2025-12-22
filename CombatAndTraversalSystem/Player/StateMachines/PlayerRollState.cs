using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerRollState : PlayerBaseState
    {
        public PlayerRollState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string ROLL_TAG = "Roll";
        private Vector3 direction;
        private float normalizedTime;

        public override void Enter()
        {
            //TODO Roll will be different, In Alert mode and in Safety mode Jump!!

            //TODO Get attack direction of enemy and, roll accordingly 
            direction = CalculateMovementDirection();

            if (direction.sqrMagnitude < 0.0001f)
            {
                stateMachine.Animator.CrossFadeInFixedTime(stateMachine.RollBackwardHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.RollForwardHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, ROLL_TAG);

            if (normalizedTime >= 1)
            {
                stateMachine.DecideTargetOrLocomotion();
                return;
            }

            Vector3 rollDirection = direction.normalized;
            Vector3 movement = rollDirection * stateMachine.RollSpeed;

            bool isAnimationActive = AnimationCalculator();
            if (!isAnimationActive) return;

            if (movement.sqrMagnitude < 0.0001f)
            {
                Vector3 backward = -stateMachine.MainCameraTransform.forward;
                backward.y = 0f;
                backward.Normalize();

                Move(backward * stateMachine.RollSpeed, deltaTime);
            }
            else
            {
                Move(movement, deltaTime);
            }
        }

        public override void Exit()
        {
        }

        private bool AnimationCalculator()
        {
            return normalizedTime >= stateMachine.RollAnimStartTime &&
                   normalizedTime <= stateMachine.RollAnimEndTime;
        }
    }
}