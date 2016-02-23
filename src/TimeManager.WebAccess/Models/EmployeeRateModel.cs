namespace Infocom.TimeManager.WebAccess.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System;
    public class EmployeeRateModel : BaseModel
    {
        #region Constructors and Destructors
        public EmployeeRateModel()
        {
            Employee = new EmployeeModel();
            StartDate = DateTime.Now.Date;
            FinishDate = new DateTime(2099,12,31);
        }
        #endregion


        #region Properties
        [DisplayName(@"Общая информация о сотруднике")]
        public EmployeeModel Employee { get; set; }

        [DisplayName(@"Начало периода действия ставки")]
        public DateTime? StartDate { get; set; }

        [DisplayName(@"Окончание периода действия ставки")]
        public DateTime? FinishDate { get; set; }

        [DisplayName(@"Часовая ставка в грн.")]
        public decimal? Rate { get; set; }
 
        [DisplayName(@"Порядковый номер")]
        public int RateNumber { get; set; }
        #endregion
    }
}