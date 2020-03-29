using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Exceptions
{
    public class InventoryCapacityException : ArgumentException
    {

        public InventoryCapacityException(string paramName, IItem item)
            : base($"Item {item.RuntimeId} ({item.Handle}) exceeds available inventory capacity", paramName)
        {

        }

    }
}
