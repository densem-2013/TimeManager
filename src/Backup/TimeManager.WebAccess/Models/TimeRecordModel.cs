namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class TimeRecordModel : BaseModel
    {
        #region Constructors and Destructors

        public TimeRecordModel()
        {
            this.StartDate = DateTime.Now;
        }

        #endregion

        #region Properties

        [DisplayName(@"Задача")]
        public long TaskID { get; set; }

        public long EmployeeID { get; set; }

        [DisplayName(@"Исполнитель")]
        public string EmployeeShortName { get; set; }

        [UIHint("Date")]
        [DisplayName(@"Дата выполнения")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime? StartDate { get; set; }

        [UIHint("Time")]
        [DisplayName(@"Время выполненния")]
        [Required(ErrorMessage = @"Не задано")]
        public TimeSpan SpentTime { get; set; }

        [DisplayName(@"Описание")]
        public string Description { get; set; }

        #endregion
    }
}