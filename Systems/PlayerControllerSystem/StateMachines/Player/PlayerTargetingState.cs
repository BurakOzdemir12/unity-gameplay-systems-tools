using _Project.Systems.PlayerControllerSystem.StateMachines.Player;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerTargetingState : PlayerBaseState
    {
        public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }


        public override void Enter()
        {
            stateMachine.InputHandler.TargetCancelEvent += OnTargetCancel;
        }

        public override void Tick(float deltaTime)
        {
            if (stateMachine.Targeter.SelectedTarget == null) return;
            Debug.Log(stateMachine.Targeter.SelectedTarget.name);
        }

        public override void Exit()
        {
            stateMachine.InputHandler.TargetCancelEvent -= OnTargetCancel;
        }

        private void OnTargetCancel()
        {
            stateMachine.Targeter.DeselectTarget();

            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }
}