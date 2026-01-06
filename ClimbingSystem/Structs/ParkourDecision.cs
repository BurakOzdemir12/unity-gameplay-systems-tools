using UnityEngine;

namespace _Project.Systems.ClimbingSystem.Structs
{
    public readonly struct ParkourDecision
    {
        public readonly bool IsValid;
        public readonly bool IsCenter;
        public readonly bool Mirror;
        public readonly AvatarTarget MatchBodyPart;

        public ParkourDecision(bool isValid, bool isCenter, bool mirror, AvatarTarget matchBodyPart)
        {
            IsValid = isValid;
            IsCenter = isCenter;
            Mirror = mirror;
            MatchBodyPart = matchBodyPart;
        }

        public static ParkourDecision Invalid => new ParkourDecision(false, false, false, AvatarTarget.LeftHand);
    }
}