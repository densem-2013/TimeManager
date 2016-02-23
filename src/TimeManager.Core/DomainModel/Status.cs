namespace Infocom.TimeManager.Core.DomainModel
{
    [PersistedEntity]
    public class Status : DomainObject
    {
        #region Properties

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual DomailEntityType ApplicableTo { get; set; }

        #endregion
    }
}