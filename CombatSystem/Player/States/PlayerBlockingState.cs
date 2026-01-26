using _Project.Systems._Core.StateMachine.Player;
using _Project.Systems.MovementSystem.Player.States;
using _Project.Systems.MovementSystem.Player.States.RootStates;
using UnityEngine;

namespace _Project.Systems.CombatSystem.Player.States
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
            stateMachine.Animator.SetBool(stateMachine.PlayerConfigSo.CombatData.IsBlockingParamHash, true);
            blockLayerWeight = stateMachine.Animator.GetLayerWeight(stateMachine.BlockingLayerIndex);
        }

        public override void Tick(float deltaTime)
        {
            float rollDampTime = stateMachine.PlayerConfigSo.CombatData.RotationDampTimeWhileBlock;

            RotateFaceToLook(deltaTime, rollDampTime);
            Move(deltaTime);
            float target = stateMachine.InputHandler.IsBlocking ? 1f : 0f;
            float normalizedTime = GetNormalizedTime(stateMachine.Animator, 1, BLOCK_TAG);

            if (!stateMachine.InputHandler.IsBlocking)
            {
                stateMachine.Animator.SetBool(stateMachine.PlayerConfigSo.CombatData.IsBlockingParamHash, false);
                blockLayerWeight =
                    Mathf.MoveTowards(blockLayerWeight, target, deltaTime * stateMachine.PlayerConfigSo.CombatData.BlockingLayerChangeSpeed);
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
                Mathf.MoveTowards(blockLayerWeight, target, deltaTime * stateMachine.PlayerConfigSo.CombatData.BlockingLayerChangeSpeed);
            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, blockLayerWeight);
        }

        public override void Exit()
        {
            stateMachine.Animator.SetBool(stateMachine.PlayerConfigSo.CombatData.IsBlockingParamHash, false);
            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, 0f);
        }
    }
}