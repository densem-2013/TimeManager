using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Validator.Constraints;

namespace Infocom.TimeManager.Core.DomainModel
{
    [PersistedEntity]
    public class RequestDetail : DomainObject
    {
        private Request _request;

        [NotNull]
        public virtual RequestDetailType RequestDetailType { get; set; }

        [NotNull]
        public virtual bool Checked { get; set; }

        public virtual Request Request //{ get; set; }
        {
            get
            {
                return this._request;
            }

            set
            {
                if (!value.RequestDetails.Contains(this))
                {
                    value.RequestDetails.Add(this);
                }

                this._request = value;
            }
        }
    }
}
