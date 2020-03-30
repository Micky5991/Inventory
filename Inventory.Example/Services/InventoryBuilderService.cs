using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example.Services
{
    public class InventoryBuilderService
    {
        private readonly IInventoryFactory _inventoryFactory;
        private readonly IItemFactory _itemFactory;

        private IInventory _inventory;

        public InventoryBuilderService(IInventoryFactory inventoryFactory, IItemFactory itemFactory)
        {
            _inventoryFactory = inventoryFactory;
            _itemFactory = itemFactory;
        }

        public async Task SetupInventoryAsync()
        {
            _inventory = _inventoryFactory.CreateInventory(5);

            await FillInventoryAsync();
        }

        private async Task FillInventoryAsync()
        {
            var item = _itemFactory.CreateItem(ItemHandle.Apple.ToString(), 1);
        }

    }
}
