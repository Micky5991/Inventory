# Custom setup

There are different ways to register needed services to the service container.

## Needed core services

There are some services which are needed, so the framework can run at all.

```cs
serviceCollection.AddInventoryServices();
```

## `IItemRegistry`

This service holds all ItemMeta instances and can be searched for available items. This is needed if any default service is in use.

```cs
serviceCollection.AddItemTypes(IItemRegistry itemRegistry);
```

## Strategy-handler

Strategy-handlers are used to handle a collection of strategies and aggregate results.

### `Micky5991.Inventory.Iterfaces.IItemMergeStrategyHandler`

```cs
// Own implementation
serviceCollection
    .AddTransient<IItemMergeStrategyHandler, ItemMergeStrategyHandler>();

// Default implementation
serviceCollection
    .AddDefaultInventoryMergeStrategy();
```

### `Micky5991.Inventory.Iterfaces.IItemSplitStrategyHandler`

```cs
// Own implementation
serviceCollection
    .AddTransient<IItemSplitStrategyHandler, ItemSplitStrategyHandler>();

// Default implementation
serviceCollection
    .AddDefaultInventorySplitStrategy();
```

## Factories

Factories are used to create instances with certain data.

### `Micky5991.Inventory.Iterfaces.IInventoryFactory`

```cs
// Own implementation
serviceCollection
    .AddTransient<IInventoryFactory, InventoryFactory>()

// Default implementation
serviceCollection
    .AddDefaultInventoryFactory();
```

### `Micky5991.Inventory.Iterfaces.IItemFactory`

```cs
// Own implementation
serviceCollection
    .AddTransient<IInventoryFactory, InventoryFactory>()

// Default implementation
serviceCollection
    .AddDefaultInventoryFactory();
```
