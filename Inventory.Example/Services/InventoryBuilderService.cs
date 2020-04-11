using System;
using System.Linq;
using Inventory.Example.Extensions;
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

        public void SetupInventory()
        {
            _inventory = _inventoryFactory.CreateInventory(10);

            this.FillInventory();
        }

        private void FillInventory()
        {
            Console.WriteLine("--> Add 1 apple and 3 water to inventory, expect 1 apple and 3 water");

            var apple = _itemFactory.CreateItem(ItemHandle.Apple, 1);
            var water = _itemFactory.CreateItem(ItemHandle.Water, 3);

            _inventory.InsertItem(apple);
            _inventory.InsertItem(water);

            foreach (var item in _inventory.Items)
            {
                Console.WriteLine($"Item: {item.Handle} - {item.GetType()} - {item.Amount}x");
            }

            Console.WriteLine("--> Add another 2 water into the inventory, expect 5 water.");

            var additionalWater = _itemFactory.CreateItem(ItemHandle.Water, 2);

            _inventory.InsertItem(additionalWater);

            foreach (var item in _inventory.Items)
            {
                Console.WriteLine($"Item: {item.Handle} - {item.GetType()} - {item.Amount}x");
            }

            Console.WriteLine("Take 2 Water from inventory and print seperate");

            var currentWater = _inventory.GetItems(ItemHandle.Water).First();
            var splitWater = currentWater.SplitItem(2);

            Console.WriteLine("[INVENTORY]");
            foreach (var item in _inventory.Items)
            {
                Console.WriteLine($"Item: {item.Handle} - {item.GetType()} - {item.Amount}x");
            }

            Console.WriteLine("[ITEM]");
            Console.WriteLine($"Item: {splitWater.Handle} - {splitWater.GetType()} - {splitWater.Amount}x");

        }

    }
}
