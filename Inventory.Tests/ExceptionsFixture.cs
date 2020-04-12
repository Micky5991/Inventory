using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ExceptionsFixture
    {

        [TestMethod]
        public void ItemNotStackableExceptionSetsMessage()
        {
            var exception = new ItemNotStackableException();

            exception.Message.Should()
                     .Contain("1 or lower")
                     .And.Contain("non-stackable");
        }

        [TestMethod]
        public void ItemNotAllowedExceptionSetsMessage()
        {
            var exception = new ItemNotAllowedException();

            exception.Message.Should()
                     .Contain("not allowed")
                     .And.Contain("item")
                     .And.Contain("inventory");
        }

    }
}
