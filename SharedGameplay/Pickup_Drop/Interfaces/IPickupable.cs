using _Project.Systems.InventorySystem.ScriptableObjects;

namespace _Project.Systems.SharedGameplay.Pickup_Drop.Interfaces
{
    public interface IPickupable
    {
        ItemData Data { get; }
        int Amount { get; }
        bool OnPickedUp();
    }
}