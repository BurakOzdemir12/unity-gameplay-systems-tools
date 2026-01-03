using _Project.Systems.ClimbingSystem.Enums;
using _Project.Systems.ClimbingSystem.Structs;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "VaultDataSo", menuName = "Scriptable Objects/Climb System/VaultDataSo")]
    public class VaultDataSo : ClimbTypeDataSo
    {
        private bool mirror;

        public override ParkourDecision Evaluate(float height, in LedgeHitData hit, Vector3 playerPos,
            Vector3 playerRight)
        {
            var baseDecision = base.Evaluate(height, in hit, playerPos, playerRight);
            if (!baseDecision.IsValid) return ParkourDecision.Invalid;

            var hitPoint = hit.ForwardRayHitInfo.transform.InverseTransformPoint(hit.ForwardRayHitInfo.point);

            mirror = hitPoint is { x: > 0f, z: > 0f } or { x: < 0f, z: < 0f };

            // Vector3 toHit = hit.ForwardRayHitInfo.point - playerPos;
            // toHit.y = 0f;
            //
            // if (toHit.sqrMagnitude < 0.0001f)
            //     return new ParkourDecision(true, false, AvatarTarget.LeftHand);
            // // side > 0 => right, side < 0 => left
            // float side = Vector3.Dot(playerRight, toHit.normalized);
            // bool mirror = side > 0f;

            var body = mirror ? AvatarTarget.RightHand : AvatarTarget.LeftHand;
            return new ParkourDecision(true, mirror, body);
        }
    }
}