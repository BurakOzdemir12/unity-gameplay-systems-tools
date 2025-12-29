using _Project.Systems.CombatAndTraversalSystem.Player.Enums;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.SuperStates;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerJumpingState : PlayerBaseState
    {
        private Vector3 momentum;
        private JumpVariant jumpType;

        private PlayerAirborneState AirborneParent => GetSuperState() as PlayerAirborneState;
        private bool findEdge;
        private const string JUMP_TAG = "Jump";
        private const string CLIMB_JUMP_TAG = "JumpClimb";
        private float normalisedTime;

        public PlayerJumpingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            findEdge = stateMachine.LedgeValidator.LockCurrentLedge();
            jumpType = findEdge ? JumpVariant.ClimbJmp : JumpVariant.Normal;


            stateMachine.ForceReceiver.ApplyJumpForce(stateMachine.JumpForce);
            momentum = stateMachine.Controller.velocity;
            momentum.y = 0;
            if (jumpType == JumpVariant.ClimbJmp && stateMachine.IsFreeFlowClimb)
            {
                stateMachine.Animator.CrossFadeInFixedTime(stateMachine.IdleToBracedHangingHash,
                    stateMachine.CrossFadeDuration);
                return;
            }

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.IdleToJumpHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            Move(momentum, deltaTime);
            if (findEdge)
            {
                var tag = stateMachine.IsFreeFlowClimb ? CLIMB_JUMP_TAG : JUMP_TAG;
                normalisedTime = GetNormalizedTime(stateMachine.Animator, 0, tag);

                if (normalisedTime > 0.5f)
                {
                    SwitchRootState(new PlayerClimbState(stateMachine));
                }

                return;
            }

            if (stateMachine.Controller.velocity.y <= stateMachine.FallingVelocityThreshold)
            {
                AirborneParent?.SwitchSubState(new PlayerFallingState(stateMachine));
                return;
            }
        }

        public override void Exit()
        {
            stateMachine.LedgeValidator.ClearLock();
        }
    }
}