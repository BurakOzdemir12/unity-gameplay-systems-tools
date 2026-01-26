using _Project.Systems._Core.Shield_Logic.Structs;
using UnityEngine;

namespace _Project.Systems._Core.Shield_Logic.Interfaces
{
    public interface IBlocker
    {
        void ApplyBlock(BlockContext context);
        bool CanBlock(Transform attackerRoot);
        bool IsBlocking { get; }
    }
}