using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using _Project.Systems.SharedGameplay.StateMachine.Player;
using UnityEngine;

namespace _Project.Systems.GatheringSystem.Player.States
{
    public class PlayerGatheringState : PlayerBaseState
    {
        public PlayerGatheringState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float moveCancelTimer;
        private PlayerGroundedState GatherParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            var animHash = stateMachine.PlayerConfigSo.GatheringDataSet.AnimHash;

            stateMachine.Animator.CrossFadeInFixedTime(animHash, stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            HandleGatherCancelByMove(deltaTime);

            RotateTowardsToResource(deltaTime);
        }

        private void HandleGatherCancelByMove(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0,
                stateMachine.PlayerConfigSo.GatheringDataSet.AnimTag);

            if (normalizedTime < 1f) return;

            float threshold = stateMachine.PlayerConfigSo.GatheringDataSet.cancelMoveThreshold;
            float holdTime = stateMachine.PlayerConfigSo.GatheringDataSet.cancelMoveHoldTime;
            bool wantsTheMove = stateMachine.InputHandler.Move.sqrMagnitude > threshold * threshold;

            if (wantsTheMove)
            {
                moveCancelTimer += deltaTime;
            }
            else
            {
                moveCancelTimer = 0f;
            }

            if (!(moveCancelTimer >= holdTime)) return;

            stateMachine.GatheringController.CancelGatherAction();
            GatherParent.SwitchSubState(new PlayerFreeLookState(stateMachine));
        }

        private void RotateTowardsToResource(float deltaTime)
        {
            Vector3 dir = stateMachine.GatheringController.CurrentResourcesData.TargetTransform.position -
                          stateMachine.transform.position;
            dir.y = 0f;

            Quaternion matchRotation = Quaternion.LookRotation(dir.normalized);
            stateMachine.transform.rotation =
                Quaternion.RotateTowards(stateMachine.transform.rotation, matchRotation, deltaTime * 180f);
        }

        public override void Exit()
        {
        }
    }
}