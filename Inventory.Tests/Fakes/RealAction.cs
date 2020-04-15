using System;
using JetBrains.Annotations;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Actions;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class RealAction : ItemActionBase<OutgoingItemActionData, IncomingItemActionData>
    {
        [CanBeNull] public Func<object, OutgoingItemActionData> ActionDataBuilder { get; set; }

        [CanBeNull] public IncomingItemActionData PassedActionData { get; private set; }

        public RealAction()
        {
        }

        public RealAction(Guid runtimeId)
            : base(runtimeId)
        {
        }

        public override void Execute(object executor, IncomingItemActionData data)
        {
            this.PassedActionData = data;
        }

        public override OutgoingItemActionData BuildActionData(object receiver)
        {
            return this.ActionDataBuilder?.Invoke(receiver);
        }
    }
}
