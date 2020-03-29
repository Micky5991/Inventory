namespace Micky5991.Inventory.Interfaces
{
    public interface IInventoryFactory
    {

        IInventory CreateInventory(int capacity);

    }
}
