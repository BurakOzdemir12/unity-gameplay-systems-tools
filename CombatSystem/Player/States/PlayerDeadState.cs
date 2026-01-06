using _Project.Systems._Core.StateMachine.Player;
namespace _Project.Systems.CombatSystem.Player.States
{
    public class PlayerDeadState : PlayerBaseState
    {

        public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, false);
            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex, 0f);

            stateMachine.Ragdoll.ToggleRagdoll(true);
            stateMachine.WeaponLogic.gameObject.SetActive(false);
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
        }
    }
}