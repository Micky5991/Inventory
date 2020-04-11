using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Micky5991.Inventory.EventArgs;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces.Strategy;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// An item is some type of stack which can be contained inside an <see cref="IInventory"/>.
    /// </summary>
    [PublicAPI]
    public interface IItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Triggers when the item has been initialized.
        /// </summary>
        event EventHandler<ItemInitializedEventArgs>? Initialized;

        /// <summary>
        /// Gets an unique handle that identifies the items meta definition.
        /// </summary>
        string Handle { get; }

        /// <summary>
        /// Gets a non persistant identifier of this item that should ONLY be used for communication in runtime and during the lifetime of this item.
        /// </summary>
        Guid RuntimeId { get; }

        /// <summary>
        /// Gets the underlying meta definition of this item, this is not changable after first initialization if <see cref="IItemRegistry"/>.
        /// </summary>
        ItemMeta Meta { get; }

        /// <summary>
        /// Gets the original name of the item given by <see cref="ItemMeta"/>.
        ///
        /// This name can be used to reset the current display name back to its original state.
        /// </summary>
        string DefaultDisplayName { get; }

        /// <summary>
        /// Gets the changable display name of this item.
        /// </summary>
        /// <seealso cref="SetDisplayName"/>
        string DisplayName { get; }

        /// <summary>
        /// Gets the positive amount of items which are represented by this instance.
        ///
        /// </summary>
        /// <seealso cref="SetAmount"/>
        int Amount { get; }

        /// <summary>
        /// Gets weight of one item in this instance of items.
        /// </summary>
        int SingleWeight { get; }

        /// <summary>
        /// Gets the total weight of this item instance. Depends on <see cref="SingleWeight"/> and <see cref="Amount"/>.
        ///
        /// </summary>
        /// <seealso cref="SingleWeight"/>
        /// <seealso cref="Amount"/>
        int TotalWeight { get; }

        /// <summary>
        /// Gets a value indicating whether this item is stackable or not.
        ///
        /// Typical characteristic of non-stackable items is the fixed <see cref="Amount"/> of 1.
        /// </summary>
        bool Stackable { get; }

        /// <summary>
        /// Gets a value indicating whether the weight of this item is changable or not.
        ///
        /// Typical characteristic of weight-changable items is that calculations with this item type without the actual item tend to be inaccurate.
        /// </summary>
        bool WeightChangable { get; }

        /// <summary>
        /// Gets current reference to the <see cref="IInventory"/> where this item is contained in.
        ///
        /// Null is equal, that this item is currently in no <see cref="IInventory"/>.
        /// </summary>
        IInventory? CurrentInventory { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the item could be moved in any way.
        /// If <see cref="Locked"/> is true, this will be true nonetheless.
        /// </summary>
        bool MovingLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether specifies any action on this item is locked.
        /// </summary>
        bool Locked { get; set; }

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
        /// Updates the current count of items in this instance to the specified <paramref name="newAmount"/>.
        ///
        /// This will change <see cref="TotalWeight"/>.
        ///
        /// </summary>
        /// <seealso cref="TotalWeight"/>
        /// <param name="newAmount">Updated amount of items this stack should represent.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="newAmount"/> is too low.</exception>
        /// <exception cref="ItemNotStackableException"><paramref name="newAmount"/> is too high for a non-stackable item.</exception>
        void SetAmount(int newAmount);

        /// <summary>
        /// Increases the total <see cref="Amount"/> by <paramref name="amountIncrease"/>.
        /// </summary>
        /// <param name="amountIncrease">Value to increase the total <see cref="Amount"/> by.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amountIncrease"/> is 0 or lower.</exception>
        /// <exception cref="ItemNotStackableException"><paramref name="amountIncrease"/> is too high for a non-stackable item.</exception>
        void IncreaseAmount(int amountIncrease);

        /// <summary>
        /// Reduces the total <see cref="Amount"/> by <paramref name="amountReduce"/>.
        /// </summary>
        /// <param name="amountReduce">Value tu reduce the total <see cref="Amount"/> by.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amountReduce"/> is higher than <see cref="Amount"/> or 0 or lower.</exception>
        void ReduceAmount(int amountReduce);

        /// <summary>
        /// Sets the single amount weight of this item.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="weight"/> is 0 or lower.</exception>
        /// <param name="weight">New value to set this item to.</param>
        void SetSingleWeight(int weight);

        /// <summary>
        /// Sets the current displayname of this item to the given <paramref name="displayName"/>.
        ///
        /// This method updates <see cref="DisplayName"/>.
        /// </summary>
        /// <seealso cref="DefaultDisplayName"/>
        /// <param name="displayName">New value to set the <see cref="DisplayName"/> to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="displayName"/> is null, empty or whitespace.</exception>
        void SetDisplayName(string displayName);

        /// <summary>
        /// Determines if the current item can be merged with the <paramref name="sourceItem"/>.
        /// </summary>
        /// <seealso cref="MergeItem"/>
        /// <param name="sourceItem">Item to check if merging would work with the current item.</param>
        /// <returns>true if the merge process would succeed and is different from the current instance, false otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sourceItem"/> is null.</exception>
        bool CanMergeWith([NotNull] IItem sourceItem);

        /// <summary>
        /// Actual implementation of the item merging process.
        ///
        /// This merges the source item onto the current instance, so the given <paramref name="sourceItem"/> will be disposed.
        /// </summary>
        /// <seealso cref="CanMergeWith"/>
        /// <param name="sourceItem">Item to merge into this.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sourceItem"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="sourceItem"/> is already this instance, <paramref name="sourceItem"/> is not mergable with this item.</exception>
        void MergeItem([NotNull] IItem sourceItem);

        /// <summary>
        /// Splits the current item into two items and returns the created one.
        ///
        /// The current amount of this item will be deducted by <paramref name="targetAmount"/>.
        /// To specify how to split the item, a <see cref="IItemSplitStrategy"/> can be specified in <see cref="Initialize"/>.
        /// </summary>
        /// <param name="targetAmount">Amount of created item.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetAmount"/> is 0 or lower or equal or higher than <see cref="Amount"/>.</exception>
        /// <returns>Newly created item.</returns>
        IItem SplitItem(int targetAmount);
    }
}
