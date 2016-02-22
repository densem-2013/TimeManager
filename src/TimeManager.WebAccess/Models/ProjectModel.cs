namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ProjectModel : BaseModel
    {
        #region Constructors and Destructors

        public ProjectModel()
        {
            this.Contract = new ContractModel();
            this.Request = new RequestModel();
            this.ProjectManager = new EmployeeModel();
            this.AssignedEmployeeIDs = new List<long>();
           
        }

        #endregion

        #region Properties

        [DisplayName(@"Наименование")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(1000)]
        public string Name { get; set; }

        [DisplayName(@"Код проекта")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(255)]
        public string Code { get; set; }

        [DisplayName(@"Учетная cтавка")]
        public decimal? Rate { get; set; }

        [DisplayName(@"Описание")]
        [StringLength(1000)]
        public string Description { get; set; }

      //  [DisplayName(@"Дата начала работ")]
      //  [Required(ErrorMessage = @"Не задано")]
      //  public DateTime StartDate { get; set; }

        [DisplayName(@"Статус проекта")]
        public long CurrentStatusID { get; set; }

        [DisplayName(@"Статус проекта")]
        public string CurrentStatusName { get; set; }
        
        [DisplayName(@"Тип проекта")]
        public long ProjectTypeID { get; set; }

        [DisplayName(@"Тип проекта")]
        public string ProjectTypeName { get; set; }

        [DisplayName(@"Приоритет")]
        public long ProjectPiorityID { get; set; }

        [DisplayName(@"Приоритет")]
        public string ProjectPiorityName { get; set; }

        public ContractModel Contract { get; set; }

        public RequestModel Request { get; set; } 

        [DisplayName(@"Руководитель проекта в КД")]
        public EmployeeModel ProjectManager { get; set; }

        [DisplayName(@"Назначеные сотрудники")]
        public IEnumerable<long> AssignedEmployeeIDs { get; set; }

        [DisplayName(@"Количество открытых задач")]
        public int OpenedTasks { get; set; }

        [DisplayName(@"Количество закрытых задач")]
        public int ClosedTasks { get; set; }
        
        [DisplayName(@"Количество задач")]
        public int AmountTasks { get; set; }

        [DisplayName(@"Идентификатор заявки")]
        public long RequestID { get; set; }

        [DisplayName(@"№ заявки")]
        public string RequestNumber { get; set; }

        [DisplayName(@"Идентификатор фазы")]
        public long PhaseID { get; set; }

        [DisplayName(@"Дата окнчания")]
        public DateTime DeadLine { get; set; }

       
       #endregion
    }
}