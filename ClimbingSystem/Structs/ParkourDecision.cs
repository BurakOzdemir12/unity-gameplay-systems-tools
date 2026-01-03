using UnityEngine;

namespace _Project.Systems.ClimbingSystem.Structs
{
    public readonly struct ParkourDecision
    {
        public readonly bool IsValid;
        public readonly bool Mirror;
        public readonly AvatarTarget MatchBodyPart;

        public ParkourDecision(bool isValid, bool mirror, AvatarTarget matchBodyPart)
        {
            IsValid = isValid;
            Mirror = mirror;
            MatchBodyPart = matchBodyPart;
        }

        public static ParkourDecision Invalid => new ParkourDecision(false, false, AvatarTarget.LeftHand);
    }
}