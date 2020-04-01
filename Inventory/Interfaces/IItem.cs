using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IItem : INotifyPropertyChanged
    {

        /// <summary>
        /// Unique handle that identifies the items meta definition.
        /// </summary>
        string Handle { get; }

        /// <summary>
        /// Non persistant identifier of this item that should ONLY be used for communication in runtime and during the lifetime of this item.
        /// </summary>
        Guid RuntimeId { get; }

        /// <summary>
        /// Underlying meta definition of this item, this is not changable after first initialization if <see cref="IItemRegistry"/>.
        /// </summary>
        ItemMeta Meta { get; }

        /// <summary>
        /// This display name is the original name of the item given by <see cref="ItemMeta"/>.
        ///
        /// This name can be used to reset the current display name back to its original state.
        /// </summary>
        string DefaultDisplayName { get; }

        /// <summary>
        /// Changable display name of this item.
        /// </summary>
        /// <seealso cref="SetDisplayName"/>
        string DisplayName { get; }

        /// <summary>
        /// Positive amount of items that are represented by this instance.
        ///
        /// </summary>
        /// <seealso cref="SetAmount"/>
        int Amount { get; }

        /// <summary>
        /// Weight of one item in this instance of items.
        /// </summary>
        int SingleWeight { get; }

        /// <summary>
        /// Total weight of this item instance. Depends on <see cref="SingleWeight"/> and <see cref="Amount"/>.
        ///
        /// </summary>
        /// <seealso cref="SingleWeight"/>
        /// <seealso cref="Amount"/>
        int TotalWeight { get; }

        /// <summary>
        /// Determines if this item is stackable or not.
        ///
        /// Typical characteristic of non-stackable items is the fixed <see cref="Amount"/> of 1.
        /// </summary>
        bool Stackable { get; }

        /// <summary>
        /// Current reference to the <see cref="IInventory"/> where this item is contained in.
        ///
        /// Null is equal, that this item is currently in no <see cref="IInventory"/>.
        /// </summary>
        IInventory? CurrentInventory { get; }

        /// <summary>
        /// Setups the item right after its creation for any handling that is not possible in a virtual constructor.
        /// </summary>
        void Initialize();

        /// <summary>
        /// /!\ INTERNAL /!\
        /// This methods updates the value of <see cref="CurrentInventory"/> to the current inventory.
        ///
        /// Use <see cref="IInventory"/> to set the item to the correct instance.
        /// </summary>
        /// <param name="inventory">Value to set the property <see cref="CurrentInventory"/> to.</param>
        void SetCurrentInventory(IInventory? inventory);

        /// <summary>
        /// Updates the current count of items in this instance to the specified <see cref="newAmount"/>.
        ///
        /// This will change <see cref="TotalWeight"/>.
        ///
        /// </summary>
        /// <seealso cref="TotalWeight"/>
        /// <param name="newAmount"></param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="newAmount"/> is too low</exception>
        void SetAmount(int newAmount);

        /// <summary>
        /// Sets the current displayname of this item to the given <paramref name="displayName"/>.
        ///
        /// This method updates <see cref="DisplayName"/>.
        /// </summary>
        /// <seealso cref="DefaultDisplayName"/>
        /// <param name="displayName">New value to set the <see cref="DisplayName"/> to</param>
        /// <exception cref="ArgumentNullException"><paramref name="displayName"/> is null, empty or whitespace</exception>
        void SetDisplayName(string displayName);

        /// <summary>
        /// Determines if the current item can be merged with the <paramref name="sourceItem"/>.
        /// </summary>
        /// <seealso cref="MergeItemAsync"/>
        /// <param name="sourceItem">Item to check if merging would work with the current item.</param>
        /// <returns>true if the merge process would succeed and is different from the current instance, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sourceItem"/> is null</exception>
        bool CanMergeWith([NotNull] IItem sourceItem);

        /// <summary>
        /// Actual implementation of the item merging process.
        ///
        /// This merges the source item onto the current instance, so the given <paramref name="sourceItem"/> will be disposed.
        /// </summary>
        /// <seealso cref="CanMergeWith"/>
        /// <param name="sourceItem">Item to merge into this</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourceItem"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="sourceItem"/> is already this instance, <paramref name="sourceItem"/> is not mergable with this item</exception>
        Task MergeItemAsync([NotNull] IItem sourceItem);

    }
}
