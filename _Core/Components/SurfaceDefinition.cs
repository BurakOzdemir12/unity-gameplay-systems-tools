using _Project.Systems._Core.Enums;
using UnityEngine;

namespace _Project.Systems._Core.Components
{
    public class SurfaceDefinition : MonoBehaviour
    {
        [SerializeField] private SurfaceType surfaceType;
        public SurfaceType SurfaceType => surfaceType;
    }
}
