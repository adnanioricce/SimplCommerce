namespace SimplCommerce.Infrastructure.Models
{
    public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
    {
        public virtual TId Id { get; protected set; }
        public virtual void SetId(TId id)
        {
            Id = id;
        }
    }
}
