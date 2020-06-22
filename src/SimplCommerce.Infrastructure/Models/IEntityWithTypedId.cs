namespace SimplCommerce.Infrastructure.Models
{
    public interface IEntityWithTypedId<TId>
    {
        TId Id { get; }
        void SetId(TId id);
    }
}
