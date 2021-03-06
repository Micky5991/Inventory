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
        [CanBeNull] public object PassedExecutor { get; private set; }
        [CanBeNull] public object PassedReceiver { get; private set; }

        public int ExecutedAmount { get; private set; }

        public RealAction()
        {
        }

        public RealAction(Guid runtimeId)
            : base(runtimeId)
        {
        }

        public override void Execute(object executor, IncomingItemActionData data)
        {
            this.PassedExecutor = executor;
            this.PassedActionData = data;

            this.ExecutedAmount++;
        }

        public override OutgoingItemActionData BuildActionData(object receiver)
        {
            this.PassedReceiver = receiver;

            return this.ActionDataBuilder?.Invoke(receiver);
        }
    }
}
