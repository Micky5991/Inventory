using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IItemFactory
    {

        IItem CreateItem(string handle, int amount);

    }
}
