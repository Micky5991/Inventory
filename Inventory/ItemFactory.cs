using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal class ItemFactory : IItemFactory
    {

        public ItemFactory(IItemRegistry registry)
        {

        }

        public IItem CreateItem(string handle, int amount)
        {
            throw new System.NotImplementedException();
        }

        private IItem BuildItemFromMeta(ItemMeta meta)
        {
            throw new System.NotImplementedException();
        }
    }
}
