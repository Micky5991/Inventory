using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.EventArgs
{
    /// <summary>
    /// Event arguments for the <see cref="IItem"/> event Initialized.
    /// </summary>
    public class ItemInitializedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInitializedEventArgs"/> class.
        /// </summary>
        /// <param name="item">Instance of <see cref="IItem"/> that has been initialized.</param>
        public ItemInitializedEventArgs(IItem item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets the reference to the item that has been initialized.
        /// </summary>
        public IItem Item { get; }
    }
}
