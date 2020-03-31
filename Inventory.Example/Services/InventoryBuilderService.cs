using System;
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
            _inventory = _inventoryFactory.CreateInventory(10);

            await FillInventoryAsync();
        }

        private async Task FillInventoryAsync()
        {
            Console.WriteLine("--> Add 1 apple and 3 water to inventory, expect 1 apple and 3 water");

            var apple = _itemFactory.CreateItem(ItemHandle.Apple.ToString(), 1);
            var water = _itemFactory.CreateItem(ItemHandle.Water.ToString(), 3);

            await _inventory.InsertItemAsync(apple);
            await _inventory.InsertItemAsync(water);

            foreach (var item in _inventory.Items.Values)
            {
                Console.WriteLine($"Item: {item.Handle} - {item.GetType()} - {item.Amount}x");
            }

            Console.WriteLine("--> Add another 2 water into the inventory, expect 5 water.");

            var additionalWater = _itemFactory.CreateItem(ItemHandle.Water.ToString(), 2);

            await _inventory.InsertItemAsync(additionalWater);

            foreach (var item in _inventory.Items.Values)
            {
                Console.WriteLine($"Item: {item.Handle} - {item.GetType()} - {item.Amount}x");
            }
        }

    }
}
