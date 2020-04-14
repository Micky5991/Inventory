# Installation

## Requirements

The following requirements and libraries are needed to use this inventory framework.

- .NET Standard 2.0
    - Look into the [.NET Standard implementation support table](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support) to see which platforms are supported.
- Microsoft.Extensions.DependencyInjection 3.1
    - You can also use compatible libraries that implement interfaces of `Microsoft.Extensions.DependencyInjection`.

## Recommendation

For improved unit-testability and flexibility you should only use [provided interfaces](api/Micky5991.Inventory.Interfaces.yml) of this library.

## Default setup

To setup needed dependencies use the provided extensions on instances that implement `IServiceCollection`.

### Setup item-registry

In order to create available item-meta definitions, you have to create a class that inherits from `Micky5991.Inventory.BaseItemRegistry`:

```cs
using System.Collections.Generic;
using Some.Application.Items;
using Micky5991.Inventory;
using Micky5991.Inventory.Enums;

namespace Some.Application
{
    public class ItemRegistry : BaseItemRegistry
    {
        protected override IEnumerable<ItemMeta> LoadItemMeta()
        {
            // List unique entries which items are available to the inventory system.

            yield return this.CreateItemMeta<AppleItem>("apple", "Apple");
            yield return this.CreateItemMeta<WaterItem>("water", "Water");
            yield return this.CreateItemMeta<DiceItem>("dice", "Dice", flags: ItemFlags.NotStackable);
        }
    }
}
```

### Register services

If you do not want to create own factories, inventory or strategyhandlers, just use the general extension:

```cs
/// ...

IServiceCollection serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

serviceCollection
    .AddDefaultInventoryServices() // Add all default services to service container
    .AddItemTypes(new Some.Application.ItemRegistry()); // Load and setup container for all available items.

/// ...
```

### Custom setup

To setup custom services and provide own ways how the framework reacts, just read about the [custom setup](Custom-Setup.md).
