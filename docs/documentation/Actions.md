# Actions

This framework offers a way to create actions which can be used to execute them on their related items.
To show that an item is compatible with the built in actions use the `IActionableItem<TOut, TIn>` interface or inherit your item class from `ActionableItem<TOut, TIn>` instead of `Item`.

> **Recommendation:** Create an abstract class to base all your items from, so extending your custom classes will be
> much easier!

## Data

To communicate with an user interface all actions offer a way to collect payloads from the actions of an item.
The base classes for data from or to actions should be inherited from `IncomingItemActionData` and `OutgoingItemActionData`.

## Create Action classes

Actions offer a convenient way to design flexible interfaces or ways to interact with your items without the
need to create each item from scratch.

> **Recommendation:** Create an abstract class to base all your actions from, so extending your custom classes will be
> much easier!

**Examples**

There are different kinds of actions you could create, for example:

- Use item
- Dispose item
- Process item (If you have some kind of Ore-Item and you are standing near an processing-tool in a game)
- Combine item with other items in the same inventory
- Shuffle item (If your item is a card deck and you want to reset its internal card stack)

> **Tip:** If you just create basic types of actions you gain the ability to style your actions from the type
> of action. Add some callbacks with input-parameters and offer the user input options. Or a displayname with description to show further details.

**Class implementation**

```cs
using System;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Actions;

namespace Some.Application.Actions
{
    public class UseAction : ItemActionBase<OutgoingItemActionData, IncomingItemActionData>
    {
        private readonly string displayName;

        private readonly Action callback;

        public UseAction(string displayName, Action callback)
        {
            this.displayName = displayName;
            this.callback = callback;
        }

        public override void Execute(IncomingItemActionData data)
        {
            this.callback();
        }

        public override OutgoingItemActionData BuildActionData()
        {
            return new OutgoingItemActionData(this.RuntimeId);
        }
    }
}
```

## Bind actions to items

Actions are bound to items when you return a list of action instances from the `RegisterAllActions()` after inheriting from `ActionableItem<TOut, TIn>`.

```cs
using System.Collections.Generic;
using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Item.Subtypes;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example.Items
{
    public class WaterItem : ActionableItem<OutgoingItemActionData, IncomingItemActionData>
    {
        public WaterItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }

        protected override IEnumerable<IItemAction<OutgoingItemActionData, IncomingItemActionData>> RegisterAllActions()
        {
            // Return an instance per action here

            yield return new ExampleAction("Action title");
        }
    }
}
```

## Executing actions

Items that implement the `IActionableItem<TOut, TIn>` interface offer the `ExecuteAction(TIn data)` method.
If you pass a payload of the type `IncomingItemActionData`, it will call the `Execute(TIn data)` method in the
correct action, if the action is enabled.

```
item.ExecuteAction(TIn data);
```
