using System;
using System.Collections.Generic;
using Inventory.Example.Actions;
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
            yield return new UseAction("Drink water", OnDrinkItem);
        }

        private void OnDrinkItem()
        {
            Console.WriteLine("Item has been consumed");
        }
    }
}
