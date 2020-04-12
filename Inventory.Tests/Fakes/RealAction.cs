using System;
using JetBrains.Annotations;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Actions;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class RealAction : ItemActionBase<OutgoingItemActionData, IncomingItemActionData>
    {
        [CanBeNull] public Func<OutgoingItemActionData> ActionDataBuilder { get; set; }

        [CanBeNull] public IncomingItemActionData PassedActionData { get; private set; }

        public RealAction()
        {
        }

        public RealAction(Guid runtimeId)
            : base(runtimeId)
        {
        }

        public override void Execute(IncomingItemActionData data)
        {
            this.PassedActionData = data;
        }

        public override OutgoingItemActionData BuildActionData()
        {
            return this.ActionDataBuilder?.Invoke();
        }
    }
}
