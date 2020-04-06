using System;

namespace Micky5991.Inventory.Exceptions
{
    public class ItemNotAllowedException : Exception
    {

        public ItemNotAllowedException(string message) : base(message)
        {

        }

    }
}
