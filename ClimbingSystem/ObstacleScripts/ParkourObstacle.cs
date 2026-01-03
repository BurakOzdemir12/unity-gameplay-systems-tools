using _Project.Systems.ClimbingSystem.Enums;
using UnityEngine;

namespace _Project.Systems.ClimbingSystem.ObstacleScripts
{
    public class ParkourObstacle : MonoBehaviour
    {
        [SerializeField] private ParkourActionType actionType;
        public ParkourActionType ActionType => actionType;
    }
}