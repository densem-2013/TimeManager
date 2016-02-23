namespace Infocom.TimeManager.Core.DomainModel
{
    using System;

    [PersistedEntity]
    public class Customer : DomainObject
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

    }
}
