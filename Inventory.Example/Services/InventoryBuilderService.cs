using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example.Services
{
    public class InventoryBuilderService
    {
        private readonly IInventoryFactory _inventoryFactory;

        private IInventory _inventory;

        public InventoryBuilderService(IInventoryFactory inventoryFactory)
        {
            _inventoryFactory = inventoryFactory;
        }

        public async Task SetupInventoryAsync()
        {
            _inventory = _inventoryFactory.CreateInventory(5);

            await FillInventoryAsync();
        }

        private async Task FillInventoryAsync()
        {
            // TODO: Fill inventory with items
        }

    }
}
