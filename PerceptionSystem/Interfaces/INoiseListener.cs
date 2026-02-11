using _Project.Systems.PerceptionSystem.Structs;

namespace _Project.Systems.PerceptionSystem.Interfaces
{
    public interface INoiseListener
    {
        void OnNoiseDetected(NoiseData noiseData);
    }
}