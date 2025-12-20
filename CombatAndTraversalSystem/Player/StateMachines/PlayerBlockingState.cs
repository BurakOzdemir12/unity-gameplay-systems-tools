using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerBlockingState : PlayerBaseState
    {
        public PlayerBlockingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float blockLayerWeight;
        private const string BLOCK_TAG = "Block";

        public override void Enter()
        {
            stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, true);
            blockLayerWeight = stateMachine.Animator.GetLayerWeight(stateMachine.BlockingLayerIndex);
        }

        public override void Tick(float deltaTime)
        {
            FaceAttackToLook(deltaTime);
            Move(deltaTime);
            float target = stateMachine.InputHandler.IsBlocking ? 1f : 0f;
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 1, BLOCK_TAG);

            if (!stateMachine.InputHandler.IsBlocking)
            {
                stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, false);
                blockLayerWeight =
                    Mathf.MoveTowards(blockLayerWeight, target, deltaTime * stateMachine.BlockingLayerChangeSpeed);
                stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, blockLayerWeight);

                if (normalizedTime >= 1f)
                {
                    DecideTargetOrLocomotion();
                }

                return;
            }

            blockLayerWeight =
                Mathf.MoveTowards(blockLayerWeight, target, deltaTime * stateMachine.BlockingLayerChangeSpeed);
            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, blockLayerWeight);
        }

        public override void Exit()
        {
            stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, false);
            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, 0f);
        }
    }
}