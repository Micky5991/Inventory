using System;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory.Tests
{
    public class ItemTest
    {
        protected const string ItemHandle = "testhandle";
        protected const string ItemDisplayName = "RealItem";
        protected const int ItemWeight = 50;
        protected const ItemFlags ItemFlags = Enums.ItemFlags.None;

        protected const string FakeItemHandle = "fakehandle";
        protected const string FakeItemDisplayName = "FakeItem";
        protected const int FakeItemWeight = 50;
        protected const ItemFlags FakeItemFlags = Enums.ItemFlags.None;

        protected ItemMeta _realMeta;
        protected ItemMeta _fakeMeta;

        protected ItemMeta _defaultRealMeta;
        protected ItemMeta _defaultFakeMeta;

        protected Item _item;
        protected FakeItem _fakeItem;

        protected IServiceCollection _serviceCollection;
        protected IServiceProvider _serviceProvider;
        protected ItemRegistry _itemRegistry;
        protected IItemFactory _itemFactory;
        protected AggregatedItemServices _itemServices;

        protected void SetupItemTest()
        {

            _itemRegistry = new ItemRegistry();

            _defaultRealMeta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            _defaultFakeMeta = new ItemMeta(FakeItemHandle, typeof(FakeItem), FakeItemDisplayName, FakeItemWeight, FakeItemFlags);

            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddInventoryServices();

        }

        protected void TearDownItemTest()
        {
            _serviceProvider = null;
            _serviceCollection = null;
            _itemFactory = null;

            _realMeta = null;
            _fakeMeta = null;

            _item = null;
            _fakeItem = null;
        }

        protected void SetupServiceProvider(params ItemMeta[] itemMetas)
        {
            _realMeta = null;
            _fakeMeta = null;

            var index = 0;
            foreach (var itemMeta in itemMetas)
            {
                _itemRegistry.AddItemMeta(itemMeta);

                switch (index)
                {
                    case 1:
                        _fakeMeta = itemMeta;
                        break;

                    case 0:
                    default:
                        _realMeta = itemMeta;

                        break;
                }

                index++;
            }

            _serviceCollection.AddItemTypes(_itemRegistry);
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _itemFactory = _serviceProvider.GetRequiredService<IItemFactory>();
            _itemServices = _serviceProvider.GetRequiredService<AggregatedItemServices>();

            _item = (Item) _itemFactory.CreateItem(_realMeta, 1);

            if (_fakeMeta != null)
            {
                _fakeItem = (FakeItem) _itemFactory.CreateItem(_fakeMeta, 1);
            }
        }

        protected void SetupDefaultServiceProvider()
        {
            SetupServiceProvider(_defaultRealMeta, _defaultFakeMeta);
        }

    }
}
