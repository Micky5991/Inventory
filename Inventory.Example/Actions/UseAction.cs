using System;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Actions;

namespace Inventory.Example.Actions
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

        public override void Execute(object executor, IncomingItemActionData data)
        {
            this.callback();
        }

        public override OutgoingItemActionData BuildActionData(object receiver)
        {
            return new OutgoingItemActionData(this.RuntimeId);
        }
    }
}
