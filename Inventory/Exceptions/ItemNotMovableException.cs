using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Exceptions
{
    public class ItemNotMovableException : Exception
    {

        public ItemNotMovableException(string message) : base(message)
        {

        }

        public ItemNotMovableException(IItem item) : base($"The item {item.Handle} ({item.RuntimeId}) is not movable")
        {

        }

    }
}
