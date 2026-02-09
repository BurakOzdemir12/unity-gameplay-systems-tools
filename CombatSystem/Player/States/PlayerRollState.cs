using _Project.Systems.CombatSystem.ScriptableObjects.Combat.Dodge_Roll;
using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerRollState : PlayerBaseState
    {
        public PlayerRollState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private const string ROLL_TAG = "Roll";

        private Vector3 direction;
        private float normalizedTime;

        private bool isTargeting;
        private bool useRootMotion;
        private RollDataSo data;
        private PlayerGroundedState GroundedParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            //TODO Roll will be different, In Alert mode and in Safety mode

            //TODO Get attack direction of enemy and, Roll accordingly prevent get damage While enemy attack
            //TODO with different enemy types, some will damage

            data = stateMachine.PlayerConfigSo.RollData;

            // isTargeting = stateMachine.PreviousState is PlayerTargetingState;
            isTargeting = stateMachine.PreviousLeafState is PlayerTargetingState;

            useRootMotion = stateMachine.workWithRootMotion;

            Vector2 input = stateMachine.InputHandler.Move;
            direction = CalculateMovementDirection();

            stateMachine.Animator.applyRootMotion = useRootMotion;

            int rollHash = GetRollHash(input, isTargeting, useRootMotion);
            stateMachine.Animator.CrossFadeInFixedTime(rollHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, ROLL_TAG);
            stateMachine.GroundChecker.IsOverridden = true;
            
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

            if (!IsRollMoveWindowActive()) return;

            Vector3 rollDir = direction.normalized;
            Vector3 movement = rollDir * data.RollSpeed;

            float rollDampTime = data.RotationDampTimeWhileRoll;

            if (movement.sqrMagnitude < 0.0001f)
            {
                Vector3 backward = -stateMachine.MainCameraTransform.forward;
                backward.y = 0f;
                backward.Normalize();

                Move(backward * data.RollSpeed, deltaTime);
            }
            else
            {
                // RotateFaceToLook(deltaTime, rollDampTime);
                Move(movement, deltaTime);
            }
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
            stateMachine.GroundChecker.IsOverridden = false;
        }

        private bool IsRollMoveWindowActive()
        {
            return normalizedTime >= data.RollAnimStartTime &&
                   normalizedTime <= data.RollAnimEndTime;
        }

        private int GetRollHash(Vector2 input, bool targeting, bool rootMotion)
        {
            if (input.sqrMagnitude < 0.0001f)
                return data.RollBackwardHash;

            if (targeting)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    return input.x > 0f ? data.RollRightHash : data.RollLeftHash;

                return input.y > 0f ? data.RollForwardHash : data.RollBackwardHash;
            }

            return data.RollForwardHash;
        }
    }
}