using System;

namespace Micky5991.Inventory.Exceptions
{
    public class ItemNotStackableException : Exception
    {

        public ItemNotStackableException()
            : base("This operation is not possible, because non-stackable items have to have an mount of 1 or lower")
        {

        }

    }
}
