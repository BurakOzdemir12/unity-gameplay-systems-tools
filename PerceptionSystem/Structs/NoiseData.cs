using UnityEngine;

namespace _Project.Systems.PerceptionSystem.Structs
{
    public struct NoiseData
    {
        public Vector3 Position;
        public GameObject Source;
        public float Range;

        public NoiseData(Vector3 position, GameObject source, float range)
        {
            Position = position;
            Source = source;
            Range = range;
        }
    }
}