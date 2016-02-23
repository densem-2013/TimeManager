namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;

    using NHibernate.Validator.Constraints;

    [PersistedEntity]
    public class Request: DomainObject
    {
        #region Constants and Fields

        private ICollection<RequestDetail> _requestDetails;
        private ICollection<RequestDetail> _requestDetailsWrapper;
        private Status _currentStatus;
        
        #endregion

        #region Constructors and Destructors

        public Request()
        {
            this.Contract = new Contract();
            this.Type = new RequestType();
            this.Phases = new List<Phase>();
            //this.RequestDetails = new List<RequestDetail>();
            this._requestDetails = new List<RequestDetail>();
            
        }

     

        #endregion

        #region Properties

        [NotNull]
        public virtual string Number { get; set; }

        [NotNull]
        public virtual DateTime Date { get; set; }

        [NotNull]
        public virtual RequestType Type { get; set; }

        [NotNull]
        public virtual Employee ProjectManager { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual ICollection<Phase> Phases { get; set; }

        public virtual Contract Contract { get; set; }

        //[NotNull]
        public virtual DateTime? CompletionDate { get; set; }

        [NotNull]
        public virtual Boolean IsValidated { get; set; }

        public virtual string Detail { get; set; }

        public virtual string Documentation { get; set; }

        public virtual Employee LastUpdateUser { get; set; }

        public virtual DateTime? LastUpdateDate { get; set; }

       // public virtual ICollection<RequestDetail> RequestDetails { get; set; }

        public virtual ICollection<RequestDetail> RequestDetails //{ get; set; }
        {
            get
            {
                return this._requestDetailsWrapper ??
                       (this._requestDetailsWrapper =
                        new LinkCollection<RequestDetail>(
                         this._requestDetails, rd => rd.Request = this, rd => rd.Request = null));
            }
            //set
            //{
            //    this._requestDetailsWrapper = value;
            //}
        }

        public virtual string TenderResult { get; set; }

        public virtual string TenderRemark { get; set; }

        public virtual Status Status
        {
            get
            {
                return this._currentStatus;
            }

            set
            {
                if (value != null)
                {
                    var applicableType = DomailEntityType.Request;
                    if (value.ApplicableTo != applicableType)
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "'{0}' is not applicable, you can assign only {1} type",
                                value.ApplicableTo,
                                applicableType));
                    }
                }

                this._currentStatus = value;
            }
        }

       

        #endregion

        
    }

}
