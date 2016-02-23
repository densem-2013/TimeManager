namespace Infocom.TimeManager.Core.DomainModel
{
    [PersistedEntity]
    public class ProjectPriority : DomainObject
    {
        #region Properties

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        #endregion

    }
}
