using System;

namespace Micky5991.Inventory.Exceptions
{
    public class ItemMetaNotFoundException : Exception
    {
        public ItemMetaNotFoundException(string message)
            : base(message)
        {
        }
    }
}
