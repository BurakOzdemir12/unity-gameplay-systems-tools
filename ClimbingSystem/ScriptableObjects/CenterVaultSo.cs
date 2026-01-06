using _Project.Systems.ClimbingSystem.Structs;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Vault_Center", menuName = "Scriptable Objects/Climb System/Vault/CenterVaultSo")]
    public class CenterVaultSo : VaultBaseSo
    {
        public override ParkourDecision Evaluate(float height, in LedgeHitData hit)
        {
            var baseDecision = base.Evaluate(height, in hit);
            if (!baseDecision.IsValid) return ParkourDecision.Invalid;

            ComputeVaultFlags(in hit, out _, out bool isCenter, out bool mirror);
            if (!isCenter) return ParkourDecision.Invalid;

            return new ParkourDecision(true, isCenter: true, mirror: mirror, matchBodyPart: MatchedBodyPart);
        }
    }
}