# Inventory

The inventory is a container that holds items. The amount of items is only limited by the inventory capacity and item weight.

## Remarks

- The inventory is a collection of items that has an arbitrary finite capacity.
- It can contain unlimited item-instances but the total weight of all items cannot exceed the inventory capacity.
- If the inventory receives individual item instances, it tries to merge them into already inserted item instances based on specified strategies defined in items.
- Inventories are able to define which items are accepted.

## InventoryFactory

To create an `IInventory` use the factory like this:

```cs
using Micky5991.Inventory.Interfaces;

IInventoryFactory inventoryFactory = serviceProvider.GetRequiredService<IInventoryFactory>();

var inventory = inventoryFactory.CreateInventory(100);
```

## Filter accepted items

The inventory can be restricted to certain items, so it prevents the insertion of non-fitting items.

> **Warning:** If you change the filter while items are already in it, the filter will only be applied to new items.

```cs
// Reset filter and accept all incoming
inventory.SetItemFilter(null);

// Restrict item to food items
inventory.SetItemFilter(item => item is IFarmingItem);
```
