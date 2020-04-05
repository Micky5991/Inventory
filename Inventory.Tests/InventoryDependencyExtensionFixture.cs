using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryDependencyExtensionFixture
    {

        private IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        private Mock<IItemRegistry> _itemRegistryMock;
        private ItemRegistry _itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            _serviceCollection = new ServiceCollection();

            _itemRegistryMock = new Mock<IItemRegistry>();
            _itemRegistry = new ItemRegistry();
        }

        [TestCleanup]
        public void Teardown()
        {
            _serviceCollection = null;
            _serviceProvider = null;
        }

        [TestMethod]
        public void RunningAddServiceExtensionsOnNullThrowsArgumentNullException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryServices(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddServicesExtensionRegistersNeededServices()
        {
            InventoryDependencyExtensions.AddDefaultInventoryServices(_serviceCollection);

            AddItemRegistryMock();
            BuildServiceProvider();

            _serviceProvider.GetRequiredService<IInventoryFactory>()
                .Should().BeOfType<InventoryFactory>();

            _serviceProvider.GetRequiredService<IItemFactory>()
                .Should().BeOfType<ItemFactory>();
        }

        [TestMethod]
        public void AddingItemRegistryOnNullServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(null, _itemRegistryMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddingNullRegistryToServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(_serviceCollection, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddItemTypesRegistersItemRegistry()
        {
            InventoryDependencyExtensions.AddItemTypes(_serviceCollection, _itemRegistry);

            BuildServiceProvider();

            _serviceProvider.GetRequiredService<IItemRegistry>().Should().Be(_itemRegistry);
        }

        [TestMethod]
        public void ReturningNullOnGetItemMetaWillThrowInvalidItemRegistryException()
        {
            _itemRegistryMock
                .Setup(x => x.GetItemMeta())
                .Returns<IEnumerable<ItemMeta>>(null);

            Action act = () => InventoryDependencyExtensions.AddItemTypes(_serviceCollection, _itemRegistryMock.Object);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("returns") && x.Message.Contains("null"));
        }

        [TestMethod]
        public void RegisteredItemsInItemRegistryWillBeResolvable()
        {
            var meta = _itemRegistry.CreateItemMetaForward<FakeItem>("item", "Fake Item");

            SetupRegistryWithItems(meta);

            InventoryDependencyExtensions.AddItemTypes(_serviceCollection, _itemRegistry);

            BuildServiceProvider();

            var factory = _serviceProvider.GetRequiredService(typeof(FakeItem));

            factory.Should()
                .NotBeNull()
                .And.BeOfType<ObjectFactory>();

            var resolvedItem = ((ObjectFactory) factory)(_serviceProvider, new [] { meta });

            resolvedItem.Should()
                .NotBeNull()
                .And.BeOfType<FakeItem>();
        }

        [TestMethod]
        public void ReturningNullAsValueInItemRegistryShouldThrowException()
        {
            _itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[] {null});

            Action act = () => InventoryDependencyExtensions.AddItemTypes(_serviceCollection, _itemRegistryMock.Object);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x =>
                    x.Message.Contains(nameof(IItemRegistry.GetItemMeta))
                    && x.Message.Contains("contains")
                    && x.Message.Contains("null")
                    && x.Message.Contains("value"));
        }

        [TestMethod]
        public void GetItemMetaWillBeCalledOnce()
        {
            _itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[0]);

            InventoryDependencyExtensions.AddItemTypes(_serviceCollection, _itemRegistryMock.Object);

            _itemRegistryMock.Verify(x => x.GetItemMeta(), Times.Once);
        }

        private void BuildServiceProvider()
        {
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        private void AddItemRegistryMock()
        {
            _serviceCollection.AddTransient(x => _itemRegistryMock.Object);
        }

        private void SetupRegistryWithItems(params ItemMeta[] metas)
        {
            foreach (var meta in metas)
            {
                _serviceCollection.AddTransient(meta.Type, x => meta);

                _itemRegistry.AddItemMeta(meta);
            }
        }

    }
}
