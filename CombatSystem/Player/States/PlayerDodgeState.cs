using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerDodgeState : PlayerBaseState
    {
        public PlayerDodgeState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string DODGE_TAG = "Dodge";
        private Vector3 direction;
        private float normalizedTime;
        private bool isTargeting;
        private bool useRootMotion;
        private PlayerGroundedState GroundedParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            //TODO Dodge will be different, In Alert mode and in Safety mode

            //TODO Get attack direction of enemy and, Dodge accordingly prevent get damage While enemy attack
            //TODO with different enemy types, some will damage

            // isTargeting = stateMachine.PreviousState is PlayerTargetingState;
            isTargeting = stateMachine.PreviousLeafState is PlayerTargetingState;

            useRootMotion = stateMachine.workWithRootMotion;

            Vector2 input = stateMachine.InputHandler.Move;
            direction = CalculateMovementDirection();

            stateMachine.Animator.applyRootMotion = useRootMotion;

            int dodgeHash = GetDodgeHash(input, isTargeting, useRootMotion);
            stateMachine.Animator.CrossFadeInFixedTime(dodgeHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, DODGE_TAG);

            if (normalizedTime >= 1f)
            {
                if (stateMachine.Targeter.SelectedTarget != null)
                {
                    GroundedParent?.SwitchSubState(new PlayerTargetingState(stateMachine));
                }
                else
                {
                    GroundedParent?.SwitchSubState(new PlayerFreeLookState(stateMachine));
                }

                // if you dont want to work with super state, you can use this line
                // stateMachine.DecideTargetOrLocomotion();
                return;
            }

            if (useRootMotion) return;
            if (!IsDodgeMoveWindowActive()) return;

            Vector3 dodgeDirection = direction.normalized;
            Vector3 movement = dodgeDirection * stateMachine.PlayerConfigSo.DodgeData.DodgeSpeed;

            float dodgeDamptime = stateMachine.PlayerConfigSo.DodgeData.RotationDampTimeWhileDodge;

            RotateFaceToLook(deltaTime, dodgeDamptime);
            if (direction.sqrMagnitude < 0.0001f)
            {
                Vector3 backward = -stateMachine.MainCameraTransform.forward;
                backward.y = 0f;
                backward.Normalize();

                Move(backward * stateMachine.PlayerConfigSo.DodgeData.DodgeSpeed, deltaTime);
            }
            else
            {
                Move(movement, deltaTime);
            }
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
        }

        private bool IsDodgeMoveWindowActive()
        {
            return normalizedTime >= stateMachine.PlayerConfigSo.DodgeData.DodgeAnimStartTime &&
                   normalizedTime <= stateMachine.PlayerConfigSo.DodgeData.DodgeAnimEndTime;
        }

        private int GetDodgeHash(Vector2 input, bool targeting, bool rootMotion)
        {
            if (input.sqrMagnitude < 0.0001f)
            {
                return stateMachine.DodgeBackwardHash;
            }

            if (targeting)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    return input.x > 0f ? stateMachine.DodgeRightHash : stateMachine.DodgeLeftHash;

                return input.y > 0f ? stateMachine.DodgeForwardHash : stateMachine.DodgeBackwardHash;
            }

            return stateMachine.DodgeForwardHash;
        }
    }
}