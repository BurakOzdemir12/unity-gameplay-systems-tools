using System;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Targeting
{
    public class Target : MonoBehaviour
    {
        public event Action<Target> OnTargetDestroyed;

        private void OnDestroy()
        {
            OnTargetDestroyed?.Invoke(this);
        }
    }
}