using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFactoryFixture
    {

        private ItemFactory _itemFactory;
        private ItemRegistry _itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            _itemRegistry = new ItemRegistry();

            _itemFactory = new ItemFactory(_itemRegistry);
        }

    }
}
