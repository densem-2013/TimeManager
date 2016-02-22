namespace Infocom.TimeManager.Core.DomainModel
{
    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class RequestType : DomainObject
    {
        [NotNull]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }
    }
}
