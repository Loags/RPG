namespace LB.Inventory
{
    public interface IAllowedItem
    {
        bool IsAllowed(ItemObject item);
    }
}