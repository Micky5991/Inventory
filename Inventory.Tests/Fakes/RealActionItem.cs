using System;
using System.Collections.Generic;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Item.Subtypes;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class RealActionItem : ActionableItem<OutgoingItemActionData, IncomingItemActionData>
    {
        public static Func<IEnumerable<IItemAction<OutgoingItemActionData, IncomingItemActionData>>> ActionGenerator
        {
            get;
            set;
        }

        public RealActionItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
        }

        public static void Reset()
        {
            ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>();
        }

        protected override IEnumerable<IItemAction<OutgoingItemActionData, IncomingItemActionData>> RegisterAllActions()
        {
            return ActionGenerator?.Invoke();
        }
    }
}
