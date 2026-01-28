using _Project.Systems.CombatSystem.Player.States;
using _Project.Systems.CombatSystem.Targeting;
using UnityEngine;

namespace _Project.Systems._Core.StateMachine.Player
{
    public abstract class PlayerBaseState : State
    {
        protected readonly PlayerStateMachine stateMachine;


        protected PlayerBaseState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
        }


        protected void Move(Vector3 movement, float deltaTime)
        {
            var motion = stateMachine.ForceReceiver.Movement;
            stateMachine.Controller.Move((motion + movement) * deltaTime);
        }

        protected void Move(float deltaTime)
        {
            Move(Vector3.zero, deltaTime);
        }

        protected Vector3 CalculateMovementDirection()
        {
            Vector2 inputValue = stateMachine.InputHandler.Move;

            Vector3 forward = stateMachine.MainCameraTransform.forward;
            Vector3 right = stateMachine.MainCameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return forward * inputValue.y +
                   right * inputValue.x;
        }

        protected bool IsReallyMoving()
        {
            return CalculateMovementDirection().sqrMagnitude > 0.0001f;
        }

        protected void RotateFaceToLook(float deltaTime, float dampTime)
        {
            if (dampTime <= 0f) return;
            if (stateMachine.Targeter.SelectedTarget != null) return;

            var targetDir = CalculateLookDirection();
            if (targetDir.sqrMagnitude < 0.0001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);

            Transform t = stateMachine.transform;
            t.rotation = Quaternion.Slerp(
                t.rotation,
                targetRotation,
                dampTime * deltaTime
            );
        }


        protected void FaceLockOnTarget(Target currentTarget, float deltaTime)
        {
            if (currentTarget == null) return;
            Vector3 targetLookPos = currentTarget.transform.position - stateMachine.transform.position;
            targetLookPos.y = 0; //if the player above or below the target we dont care..
            Quaternion targetRotation = stateMachine.transform.rotation = Quaternion.LookRotation(targetLookPos);
            Quaternion.Slerp(stateMachine.transform.rotation, targetRotation,
                (stateMachine.PlayerConfigSo.MovementData.RotationDampTime * deltaTime));
        }

        private Vector3 CalculateLookDirection()
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


        // Block while other states runs
        protected void HandleBlocking(float deltaTime, bool allowBlocking = true)
        {
            if (stateMachine.CurrentSubState is PlayerDeadState or PlayerAttackingState)
            {
                return;
            }

            // Or just use current ststae check know instead allow bool
            bool wantsBlock = allowBlocking && stateMachine.InputHandler.IsBlocking;
            
            if (wantsBlock)
            {
                stateMachine.ShieldHandler.EnableShield();
            }
            else
            {
                stateMachine.ShieldHandler.DisableShield();
            }

            stateMachine.Animator.SetBool(stateMachine.PlayerConfigSo.CombatData.IsBlockingParamHash, wantsBlock);

            float target = wantsBlock ? 1f : 0f;

            stateMachine.blockLayerWeight = Mathf.MoveTowards(
                stateMachine.blockLayerWeight,
                target,
                deltaTime * stateMachine.PlayerConfigSo.CombatData.BlockingLayerChangeSpeed
            );

            stateMachine.Animator.SetLayerWeight(
                stateMachine.BlockingLayerIndex,
                stateMachine.blockLayerWeight
            );
        }

        protected void HandleCombatLayer(float deltaTime, bool shouldBeActive)
        {
            float targetWeight = shouldBeActive ? 1f : 0f;

            // Set Animator Bool for transitions if needed
            stateMachine.Animator.SetBool(stateMachine.IsArmedBoolHash, stateMachine.isWeaponEquipped);

            stateMachine.armedLayerWeight = Mathf.MoveTowards(
                stateMachine.armedLayerWeight,
                targetWeight,
                deltaTime * 10f); // Smooth transition (10f speed)

            stateMachine.Animator.SetLayerWeight(stateMachine.ArmedLayerIndex, stateMachine.armedLayerWeight);
        }

        protected void HandleSheathDraw(bool allowSheathDraw = true)
        {
            if (stateMachine.CurrentSubState is PlayerDeadState or PlayerAttackingState)
            {
                return;
            }

            if (!allowSheathDraw) return;

            bool isWeaponInHand = stateMachine.isWeaponEquipped;

            // Set Layer weight to 1 instantly to ensure visibility
            stateMachine.upperBodyLayerWeight = 1f;
            stateMachine.Animator.SetLayerWeight(stateMachine.UpperBodyLayerIndex, 1f);
            var combatData = stateMachine.PlayerConfigSo.CombatData;
            int hash = isWeaponInHand ? combatData.SheatAnimHash : combatData.DrawAnimHash;

            stateMachine.Animator.CrossFadeInFixedTime(hash, stateMachine.CrossFadeDuration,
                stateMachine.UpperBodyLayerIndex);
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