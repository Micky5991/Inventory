using System;

namespace Micky5991.Inventory.Enums
{
    [Flags]
    public enum ItemFlags : uint
    {
        None = 0,
        NotStackable = 1,
        WeightChangable = 1 << 1
    }
}
