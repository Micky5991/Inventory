# Custom setup

There are different ways to register needed services to the service container.

## Needed core services

There are some services which are needed, so the framework can run at all.

```cs
serviceCollection.AddInventoryServices();
```

## `IItemRegistry`

This service will be registered with the following extension and there is no alternative.

```cs
serviceCollection.AddItemTypes(IItemRegistry itemRegistry);
```

## Strategy-handler

Strategy-handlers are used to handle a collection of strategies and aggregate results.

### Default handlers

This framework provides a default implementation for item-merge and item-split strategy-handlers. To use them, just call:
```cs
serviceCollection.AddDefaultInventoryStrategies()
```

### `IItemMergeStrategyHandler`

**Own implementation**
```cs
serviceCollection.AddTransient<IItemMergeStrategyHandler, ItemMergeStrategyHandler>();
```

**Default implementation**
```cs
serviceCollection.AddDefaultInventoryMergeStrategy();
```

### `IItemSplitStrategyHandler`

**Own implementation**
```cs
serviceCollection.AddTransient<IItemSplitStrategyHandler, ItemSplitStrategyHandler>();
```

**Default implementation**
```cs
serviceCollection.AddDefaultInventorySplitStrategy();
```
