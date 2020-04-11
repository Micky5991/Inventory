using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
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
            this._serviceCollection = new ServiceCollection();

            this._itemRegistryMock = new Mock<IItemRegistry>();
            this._itemRegistry = new ItemRegistry();
        }

        [TestCleanup]
        public void Teardown()
        {
            this._serviceCollection = null;
            this._serviceProvider = null;
        }

        [TestMethod]
        public void RunningAddInventoryServicesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddInventoryServices(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryStrategiesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryStrategies(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryMergeStrategyOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryMergeStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventorySplitStrategyOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventorySplitStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryFactoryOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryFactory(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultItemFactoryOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultItemFactory(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultFactoriesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultFactories(null);

            act.Should().Throw<ArgumentNullException>();
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
            InventoryDependencyExtensions.AddDefaultInventoryServices(this._serviceCollection);

            this.AddItemRegistryMock();
            this.BuildServiceProvider();

            this._serviceProvider.GetRequiredService<IInventoryFactory>()
                .Should().BeOfType<InventoryFactory>();

            this._serviceProvider.GetRequiredService<IItemFactory>()
                .Should().BeOfType<ItemFactory>();
        }

        [TestMethod]
        public void AddingItemRegistryOnNullServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(null, this._itemRegistryMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddingNullRegistryToServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddItemTypesRegistersItemRegistry()
        {
            InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, this._itemRegistry);

            this.BuildServiceProvider();

            this._serviceProvider.GetRequiredService<IItemRegistry>().Should().Be(this._itemRegistry);
        }

        [TestMethod]
        public void ReturningNullOnGetItemMetaWillThrowInvalidItemRegistryException()
        {
            this._itemRegistryMock
                .Setup(x => x.GetItemMeta())
                .Returns<IEnumerable<ItemMeta>>(null);

            Action act = () => InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, this._itemRegistryMock.Object);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("returns") && x.Message.Contains("null"));
        }

        [TestMethod]
        public void RegisteredItemsInItemRegistryWillBeResolvable()
        {
            var meta = this._itemRegistry.CreateItemMetaForward<FakeItem>("item", "Fake Item");

            this.SetupRegistryWithItems(meta);

            InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, this._itemRegistry);

            this.BuildServiceProvider();

            var factory = this._serviceProvider.GetRequiredService(typeof(FakeItem));

            factory.Should()
                .NotBeNull()
                .And.BeOfType<ObjectFactory>();

            var resolvedItem = ((ObjectFactory) factory)(this._serviceProvider, new [] { meta });

            resolvedItem.Should()
                .NotBeNull()
                .And.BeOfType<FakeItem>();
        }

        [TestMethod]
        public void ReturningNullAsValueInItemRegistryShouldThrowException()
        {
            this._itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[] {null});

            Action act = () => InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, this._itemRegistryMock.Object);

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
            this._itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[0]);

            InventoryDependencyExtensions.AddItemTypes(this._serviceCollection, this._itemRegistryMock.Object);

            this._itemRegistryMock.Verify(x => x.GetItemMeta(), Times.Once);
        }

        private void BuildServiceProvider()
        {
            this._serviceProvider = this._serviceCollection.BuildServiceProvider();
        }

        private void AddItemRegistryMock()
        {
            this._serviceCollection.AddTransient(x => this._itemRegistryMock.Object);
        }

        private void SetupRegistryWithItems(params ItemMeta[] metas)
        {
            foreach (var meta in metas)
            {
                this._serviceCollection.AddTransient(meta.Type, x => meta);

                this._itemRegistry.AddItemMeta(meta);
            }
        }

    }
}
