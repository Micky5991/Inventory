using FluentAssertions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemActionFixture : ItemTest
    {

        private RealAction action;

        [TestInitialize]
        public void Setup()
        {
            this.SetupItemTest();

            this.action = new RealAction();
        }

        [TestCleanup]
        public void Teardown()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void CreatingActionSetsCorrectRuntimeId()
        {
            this.action.RuntimeId.Should().NotBeEmpty();
        }

        [TestMethod]
        public void RuntimeIdIsCreatedEveryTime()
        {
            var otherAction = new RealAction();

            this.action.RuntimeId.Should().NotBe(otherAction.RuntimeId);
        }

        [TestMethod]
        public void IsVisibleReturnsTrueDefault()
        {
            this.action.IsVisible().Should().BeTrue();
        }

        [TestMethod]
        public void IsVisibleReturnsTrueOnNullCheck()
        {
            this.action.SetVisibleCheck(null);

            this.action.IsVisible().Should().BeTrue();
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void IsVisibleReturnsCorrectValueFromCheck(bool visible)
        {
            this.action.SetVisibleCheck(() => visible);

            this.action.IsVisible().Should().Be(visible);
        }

        [TestMethod]
        public void IsEnabledReturnsTrueDefault()
        {
            this.action.IsEnabled().Should().BeTrue();
        }

        [TestMethod]
        public void IsEnabledReturnsTrueForNullCheck()
        {
            this.action.SetEnabledCheck(null);

            this.action.IsEnabled().Should().BeTrue();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsEnabledReturnsValueReturnedInCheck(bool enabled)
        {
            this.action.SetEnabledCheck(() => enabled);

            this.action.IsEnabled().Should().Be(enabled);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void IsEnabledReturnsFalseAndIgnoresEnabledValue(bool enabled)
        {
            this.action.SetEnabledCheck(() => enabled);
            this.action.SetVisibleCheck(() => false);

            this.action.IsEnabled().Should().BeFalse();
        }

        [TestMethod]
        public void IsEnabledReturnsFalseEvenWhenNoEnabledCheckIsSet()
        {
            this.action.SetEnabledCheck(null);
            this.action.SetVisibleCheck(() => false);

            this.action.IsEnabled().Should().BeFalse();
        }

        [TestMethod]
        public void SetRelatedItemSetsProperty()
        {
            this.action.SetRelatedItem(this.Item);

            this.action.RelatedItem.Should().Be(this.Item);
        }

        [TestMethod]
        public void SetRelatedItemToNullSetsPropertyToNull()
        {
            this.action.SetRelatedItem(this.Item);
            this.action.SetRelatedItem(null);

            this.action.RelatedItem.Should().BeNull();
        }

    }
}
