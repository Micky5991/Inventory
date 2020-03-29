using System;

namespace Micky5991.Inventory
{
    public class ItemMeta
    {

        public string Handle { get; }

        public Type Type { get; }

        public int DefaultWeight { get; }

        public ItemMeta(string handle, Type type, int defaultWeight)
        {
            Handle = handle;
            Type = type;
            DefaultWeight = defaultWeight;
        }

    }
}
