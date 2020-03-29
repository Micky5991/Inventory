using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class FakeItem : IItem
    {
        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public int Weight { get; }
    }
}
