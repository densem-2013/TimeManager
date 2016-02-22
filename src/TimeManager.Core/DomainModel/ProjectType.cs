namespace Infocom.TimeManager.Core.DomainModel
{
    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class ProjectType: DomainObject
    {
        #region Properties

        public virtual string Name { get; set; }

        #endregion

    }
}
