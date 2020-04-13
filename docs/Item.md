# Item

An item instance that implements `Micky5991.Inventory.Interface.IItem` is a stack of items and it
can exist outside an inventory.

## Remarks

- An item is a virtual element that contains actions and has an amount.
- Items are stackable by default if not defined otherwise in item meta.
- Characteristics of items are defined in item meta.
- The total weight of an item is based on the factors single-amount weight and the amount. `total-weight = single-weight * amount`
- The weight of an item-instance is not changable in runtime if not defined otherwise in item meta.
- Unstackable items cannot merge with the same kind and are limited to amount of 0 or 1.
- Items have display-names which are changable in runtime at any time.
- Each item can specify a strategy on how to merge with other item instances.
    - The default merge strategy has the following requirements
        - Both items are stackable.
        - Both items are from the same kind (handle).
        - Both items have equal single-weight.

## ItemFactory

To create an item use the `IItemFactory` service as follows:

### Single item or stackable items

```cs
using Micky5991.Inventory.Interfaces;

IItemFactory itemFactory = serviceProvider.GetRequiredService<IItemFactory>();
IItemRegistry itemRegistry = serviceProvider.GetRequiredService<IItemRegistry>();

// Overload with item-handle
var apple = itemfactory.CreateItem("apple", 1);

// Overload with item-meta
if (itemRegistry.TryGetItemMeta("apple", out var meta)) {
    var apple = itemfactory.CreateItem(meta, 1);
}
```

### Non-Stackable item

If the item is not stackable and multiple items should be created, use `CreateItems`:

```cs
using Micky5991.Inventory.Interfaces;

IItemFactory itemFactory = serviceProvider.GetRequiredService<IItemFactory>();
IItemRegistry itemRegistry = serviceProvider.GetRequiredService<IItemRegistry>();

// Overload with item-handle
var apples = itemfactory.CreateItems("apple", 10);

// Overload with item-meta
if (itemRegistry.TryGetItemMeta("apple", out var meta)) {
    var apples = itemfactory.CreateItems(meta, 10);
}
```

## Add item to inventory

Any instance of `IItem` can be added to an inventory with the following method in `IInventory`:

```cs
inventory.InsertItem(IItem item);
```

## Locking

Locking is a native way to prevent item from leaving its current inventory state or to execute actions on the item.

### Complete lock

A complete lock is achieved by executing the following method and currently prevents the item from executing any actions and leaving its inventory state.

```cs
item.Locked = state;
```

### Lock movement

Movement lock prevents the item from leaving its current inventory state. The movement is also locked if the item has been locked with `item.Locked`.

```cs
item.MovingLocked = state;
```

## Weight

The item always has non-zero weight and has two weight values to work with.

The value type of the weight is integer, because it offers the most precision.
If you want to display them with decimals in your interfaces, just divide all weight-values by your value. For example if you want to display values like: `100.00`, multiply/divide by `100`.

### Single weight

- The single weight is the total weight of this item, if the amount of the item would be equal to 1.
- It is a factor in calculation for `item.TotalWeight`
- The single weight is changable if this ability is specified in `ItemMeta` (See [ItemRegistry](ItemRegistry.md)).
- Changing single weight respects the boundaries of the current inventory of the item.

```cs
// Get current single weight
item.SingleWeight

// Set current single weight (If changable)
item.SetSingleWeight(int newWeight);
```

### Total weight

- The total weight is the product of `item.Amount` and `item.SingleWeight`.
- The total weight is not changable. Only if you alter `item.Amount` or `item.SingleWeight`.

```cs
item.TotalWeight;
```

## Destroy item

To destroy an item you just have to set its amount to 0. If the amount drops to 0 it will be removed from its current inventory automatically.

```cs
item.SetAmount(0);
```

## Create own child-classes

The main advantage of this inventory framework is the ability to create custom implementations for each item and
add custom logic to each item individually.

### Basic class

To create your own implementation you just have to inherit from `Micky5991.Inventory.Entities.Item.Item` and start adding some custom behavior.

> **Recommendation:** Create an abstract class to base all your items from, so extending your custom classes will be
> much easier!

```cs
using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;

namespace Some.Application
{
    public class AppleItem : Item
    {
        public AppleItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
        }
    }
}
```

### Constructor injection

All items are resolved from the service provider, so all services you registered are available to you. To request registered services, just add them to your constructor of the item:

```cs
using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Some.Application.Interfaces;

namespace Some.Application
{
    public class AppleItem : Item
    {
        private IPlayerManager playerManager;

        public AppleItem(IPlayerManager playerManager, ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
            this.playerManager = playerManager;
        }
    }
}
```

### Custom strategies

If you want to specify custom strategies in your item to split items or merge others into this, just overload the method `SetupStrategies`:

```cs
using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Some.Application.Interfaces;

namespace Some.Application
{
    public class AppleItem : Item
    {
        // ...

        protected override void SetupStrategies()
        {
            base.SetupStrategies(); // Adds default strategies

            this.ItemMergeStrategyHandler.Add(new MyAwesomeMergeStrategy());
            this.ItemSplitStrategyHandler.Add(new MyAwesomeSplitStrategy());
        }

        // ...
    }
}
```
