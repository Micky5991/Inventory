namespace Micky5991.Inventory.Interfaces.Strategy
{
    /// <summary>
    /// Handler that executes created methods on all registered <see cref="IItemSplitStrategy"/>.
    /// </summary>
    public interface IItemSplitStrategyHandler : IStrategyHandler<IItemSplitStrategy>, IItemSplitStrategy
    {
    }
}
