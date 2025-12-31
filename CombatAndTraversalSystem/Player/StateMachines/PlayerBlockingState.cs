using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines.RootStates;
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
        private PlayerGroundedState GroundParent => GetSuperState() as PlayerGroundedState;

        public override void Enter()
        {
            stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, true);
            blockLayerWeight = stateMachine.Animator.GetLayerWeight(stateMachine.BlockingLayerIndex);
        }

        public override void Tick(float deltaTime)
        {
            float rollDampTime = stateMachine.RotationDampTimeWhileBlock;

            RotateFaceToLook(deltaTime, rollDampTime);
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
                    if (stateMachine.Targeter.SelectedTarget != null)
                    {
                        GroundParent.SwitchSubState(new PlayerTargetingState(stateMachine));
                    }
                    else
                    {
                        GroundParent.SwitchSubState(new PlayerFreeLookState(stateMachine));
                    }
                    // stateMachine.DecideTargetOrLocomotion();
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