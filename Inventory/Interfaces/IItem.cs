using System;
using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItem
    {

        string Handle { get; }

        Guid RuntimeId { get; }

        ItemMeta Meta { get; }

        string DefaultDisplayName { get; }

        string DisplayName { get; }

        int Amount { get; }

        int SingleWeight { get; }

        int TotalWeight { get; }

        bool Stackable { get; }

        IInventory? CurrentInventory { get; }

        void SetCurrentInventory(IInventory? inventory);

        void SetAmount(int newAmount);

        void SetDisplayName(string displayName);

        bool CanMergeWith(IItem sourceItem);
        Task MergeItemAsync(IItem sourceItem);

    }
}
