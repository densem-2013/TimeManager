namespace Infocom.TimeManager.Core.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.Core.DomainModel;

    [PersistedEntity]
    public class vwProjectDetailsWithSpentTimeByBudgetAllocation: DomainObject
    {

        public vwProjectDetailsWithSpentTimeByBudgetAllocation()
        {
         
        }
       
        public virtual long ProjectID { get; set; }

        public virtual TimeSpan SpentTime { get; set; }

        public virtual string ManagerShortName { get; set; }

        public virtual string EmployeeShortName { get; set; }

        public virtual DateTime TimeRecordStartDate { get; set; }

        public virtual DateTime? ContractNumberAndSigningDate { get; set; }

        public virtual string ContractCustomer { get; set; }

        public virtual DateTime ProjectDeadLine { get; set; }

        public virtual string ProjectName { get; set; }

        public virtual string ProjectOrder { get; set; }

        public virtual string DepartmentName { get; set; }

        public virtual long DepartmentID { get; set; }

        public virtual string TaskName { get; set; }

        public virtual long AmountOfHours { get; set; }

        public virtual long AmountOfMoney { get; set; }

        public virtual string StatusName { get; set; }

        public virtual string PorjectTypeName { get; set; }

         public virtual double ProjectCode { get; set; }
         public virtual double EmployeeRate { get; set; }

         public virtual DateTime? DateEndWorkByDepartment { get; set; }

    }
}