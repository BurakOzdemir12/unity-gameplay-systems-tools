using _Project.Systems.CombatAndTraversalSystem.Targeting;
using _Project.Systems.Core.StateMachine;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public abstract class PlayerBaseState : State
    {
        protected readonly PlayerStateMachine stateMachine;


        protected PlayerBaseState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void Move(Vector3 movement, float deltaTime)
        {
            var motion = stateMachine.ForceReceiver.Movement;
            stateMachine.Controller.Move((motion + movement) * deltaTime);
        }

        protected void Move(float deltaTime) //, Vector3 attackForceTargetPos, float AttackForce
        {
            // stateMachine.Controller.Move(attackForceTargetPos * (AttackForce * deltaTime));
            // var motion = stateMachine.ForceReceiver.Movement;
            //
            // stateMachine.Controller.Move(motion * deltaTime);
            Move(Vector3.zero, deltaTime);
        }

        protected void FaceAttackToLook(float deltaTime)
        {
            if (stateMachine.Targeter.SelectedTarget != null)
                return;

            var targetDir = CalculateAttackDirection();

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            Transform t = stateMachine.transform;
            t.rotation = Quaternion.Slerp(
                t.rotation,
                targetRotation,
                stateMachine.RotationDampTimeWhileAttack * deltaTime
            );
        }


        protected void FaceTarget(Target currentTarget, float deltaTime)
        {
            if (currentTarget == null) return;
            Vector3 targetLookPos = currentTarget.transform.position - stateMachine.transform.position;
            targetLookPos.y = 0; //if the player above or below the target we dont care..
            Quaternion targetRotation = stateMachine.transform.rotation = Quaternion.LookRotation(targetLookPos);
            Quaternion.Slerp(stateMachine.transform.rotation, targetRotation,
                (stateMachine.RotationDampTime * deltaTime));
        }

        private Vector3 CalculateAttackDirection()
        {
            Vector3 forward = stateMachine.MainCameraTransform.forward;
            Vector3 right = stateMachine.MainCameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector2 lookInput = stateMachine.InputHandler.Look;
            Vector3 targetDir = forward * lookInput.y + right * lookInput.x;

            if (targetDir.sqrMagnitude < 0.0001f)
                targetDir = forward;

            targetDir.y = 0f;
            return targetDir;
        }

        protected void DecideTargetOrLocomotion()
        {
            if (stateMachine.Targeter.SelectedTarget != null)
            {
                stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            }
        }

        // Block while other states runs
        protected void TickBlockingOverlay(float deltaTime, bool allowBlocking = true)
        {
            if (stateMachine.CurrentState is PlayerDeadState or PlayerAttackingState)
            {
                return;
            }

            // Or just use current ststae check know instead allow bool
            bool wantsBlock = allowBlocking && stateMachine.InputHandler.IsBlocking;

            stateMachine.Animator.SetBool(stateMachine.IsBlockingBoolHash, wantsBlock);

            float target = wantsBlock ? 1f : 0f;

            stateMachine.blockLayerWeight = Mathf.MoveTowards(
                stateMachine.blockLayerWeight,
                target,
                deltaTime * stateMachine.BlockingLayerChangeSpeed
            );

            stateMachine.Animator.SetLayerWeight(
                stateMachine.BlockingLayerIndex,
                stateMachine.blockLayerWeight
            );
        }
        //  If you want to block state by itself use Changing state code
        // protected bool TrySwitchToBlockState() 
        // {
        //     if (!stateMachine.InputHandler.IsBlocking) return false;
        //
        //     stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
        //     return true;
        // }
    }
}