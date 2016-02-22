namespace Infocom.TimeManager.Core.DomainModel
{
    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class ContractType : DomainObject
    {
        [NotNull]
        public virtual string Name { get; set; }

        public virtual string ShortName { get; set; }
    }
}
