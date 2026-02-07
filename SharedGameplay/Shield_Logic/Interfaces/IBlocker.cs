using _Project.Systems.SharedGameplay.Shield_Logic.Structs;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.Shield_Logic.Interfaces
{
    public interface IBlocker
    {
        void ApplyBlock(BlockContext context);
        bool CanBlock(Transform attackerRoot);
        bool IsBlocking { get; }
    }
}