using System;
using FluentAssertions;
using Micky5991.Inventory.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class IncomingItemActionDataFixture
    {
        [TestMethod]
        public void CreatingDataSetsCorrectValues()
        {
            var guid = Guid.NewGuid();

            var createdData = new IncomingItemActionData(guid);

            createdData.ActionRuntimeId.Should().Be(guid);
        }

        [TestMethod]
        public void IncomingActionDataInheritsRightBaseClass()
        {
            var data = new IncomingItemActionData(Guid.NewGuid());

            data.Should().BeAssignableTo<ItemActionData>();
        }

        [TestMethod]
        public void SetEmptyGuidThrowsException()
        {
            Action act = () => new IncomingItemActionData(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
