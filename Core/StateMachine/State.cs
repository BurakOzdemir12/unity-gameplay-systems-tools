using UnityEngine;

namespace _Project.Systems.Core.StateMachine
{
    public abstract class State
    {
        public abstract void Enter();
        public abstract void Tick(float deltaTime);
        public abstract void Exit();

        protected float GetNormalizedTime(Animator animator,int layerIndex, string tag)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);

            if (animator.IsInTransition(layerIndex) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(layerIndex) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
    }
}