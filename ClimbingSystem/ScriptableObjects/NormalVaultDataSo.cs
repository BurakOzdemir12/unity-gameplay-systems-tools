using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.Structs;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Vault_Normal", menuName = "Scriptable Objects/Climb System/Vault/NormalVaultSo")]
    public class NormalVaultDataSo : VaultBaseSo
    {
        public override ParkourDecision Evaluate(float height, in LedgeHitData hit)
        {
            var baseDecision = base.Evaluate(height, in hit);
            if (!baseDecision.IsValid) return ParkourDecision.Invalid;

            ComputeVaultFlags(in hit, out _, out bool isCenter, out bool mirror);

            if (isCenter) return ParkourDecision.Invalid;


            var body = mirror ? AvatarTarget.RightHand : AvatarTarget.LeftHand;
            return new ParkourDecision(true, isCenter: false, mirror: mirror, matchBodyPart: body);
        }
    }
}