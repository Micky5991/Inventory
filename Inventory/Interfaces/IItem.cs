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

        int TotalWeight { get; }

        bool Stackable { get; }

        void SetDisplayName(string displayName);

        bool CanMergeWith(IItem sourceItem);

        Task MergeItemAsync(IItem sourceItem);

    }
}
