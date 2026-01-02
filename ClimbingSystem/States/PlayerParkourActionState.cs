using _Project.Systems.ClimbingSystem.ScriptableObjects;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines;
using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.States
{
    public class PlayerParkourActionState : PlayerBaseState
    {
        private string stepUpTag;
        private readonly ClimbTypeDataSo so;
        private bool hasMatchedTarget;

        public PlayerParkourActionState(PlayerStateMachine stateMachine, ClimbTypeDataSo so) : base(stateMachine)
        {
            this.so = so;
        }

        public override void Enter()
        {
            stepUpTag = so.AnimTag;
            stateMachine.Animator.applyRootMotion = true;
            stateMachine.Controller.enabled = false;
            stateMachine.Animator.CrossFade(so.AnimHash, stateMachine.StepUpClimbCrossFadeDuration);
            hasMatchedTarget = false;
        }

        public override void Tick(float deltaTime)
        {
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 0, stepUpTag);
            if (normalizedTime > 1f)
            {
                SwitchRootState(new PlayerGroundedState(stateMachine));
            }

            if (so.rotateToObstacle && normalizedTime < 1f)
            {
                var hitData = stateMachine.ClimbController.CurrentLedgeHitData;

                if (so.rotateToObstacle)
                {
                    Vector3 normal = hitData.forwardRayHitInfo.normal;
                    Quaternion matchRotation = Quaternion.LookRotation(-normal);
                    stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation,
                        matchRotation, so.rotationSpeed * deltaTime);
                }
            }

            if (so.enableTargetMatching)
            {
                MatchTarget(normalizedTime, deltaTime);
            }
        }

        private void MatchTarget(float normalizedTime, float deltaTime)
        {
            if (stateMachine.Animator.isMatchingTarget) return;
            if (hasMatchedTarget) return;
            if (normalizedTime < so.MatchStartTime || normalizedTime > so.MatchTargetTime) return;

            var hitData = stateMachine.ClimbController.CurrentLedgeHitData;
            Vector3 matchPos = hitData.heightRayHitInfo.point;

            // Debug.Log($"[ParkourState] CALLING MatchTarget! Pos: {matchPos} | Time: {normalizedTime}");

            Vector3 soWeight = so.MatchPosWeight;
            MatchTargetWeightMask weightMask = new MatchTargetWeightMask(soWeight, 0);
            // MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 0);
            // Usually we match position (1) and maybe rotation (0 or 1). 
            // If rotation is handled by RotateToObstacle separately, we might set rotation weight to 0.

            stateMachine.Animator.MatchTarget(
                matchPos,
                stateMachine.transform.rotation,
                so.MatchedBodyPart,
                weightMask,
                so.MatchStartTime,
                so.MatchTargetTime
            );
            hasMatchedTarget = true;
        }

        public override void Exit()
        {
            stateMachine.Animator.applyRootMotion = false;
            stateMachine.Controller.enabled = true;
        }
    }
}