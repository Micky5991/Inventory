using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Entities.Inventory
{
    /// <content>
    /// Implementation of <see cref="INotifyPropertyChanged"/> interface.
    /// </content>
    public partial class Inventory
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invocator for the interface <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property that called this method.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
