using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Validator.Constraints;

namespace Infocom.TimeManager.Core.DomainModel
{
    [PersistedEntity]
    public class RequestDetailType : DomainObject
    {
        [NotNull]
        public virtual string Name  { get; set; }
    }
}
