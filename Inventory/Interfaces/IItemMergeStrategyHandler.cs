namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Handler that executes created methods on all registered <see cref="IItemMergeStrategy"/>.
    /// </summary>
    public interface IItemMergeStrategyHandler : IStrategyHandler<IItemMergeStrategy>, IItemMergeStrategy
    {
    }
}
