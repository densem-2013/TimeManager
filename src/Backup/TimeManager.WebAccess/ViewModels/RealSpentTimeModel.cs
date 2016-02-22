using System;
namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RealSpentTimeModel
    {
        [DisplayName(@"Дата выполнения")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime? StartDate { get; set; }

        [DisplayName(@"Время выполненния")]
        [Required(ErrorMessage = @"Не задано")]
        public TimeSpan SpentTime { get; set; }

        [DisplayName(@"Выходной")]
        [DefaultValue(false)]
        public bool isWeekEnd { get; set; }

    }
}
