using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IItem : INotifyPropertyChanged
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

        bool CanMergeWith([NotNull] IItem sourceItem);
        Task MergeItemAsync([NotNull] IItem sourceItem);

    }
}
