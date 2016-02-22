namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Infocom.TimeManager.WebAccess.Models;

    public class PhaseModel : DynamicBaseModel
    {
        public PhaseModel()
        {
            this.Budget = new BudgetModel();
            
        }

        [DisplayName(@"Наименование")]
        [Required(ErrorMessage = @"Не задано")]
        [StringLength(255)]
        public string Name { get; set; }

        [DisplayName(@"Дата окончания")]
        [Required(ErrorMessage = @"Не задано")]
        public DateTime? DeadLine { get; set; }

        [DisplayName(@"Распределение бюджетов")]
        [Required(ErrorMessage = @"Не задано")]
        public BudgetModel Budget{ get; set; }

        [DisplayName(@"Статус этапа")]
        public string ProcessState { get; set; }
    }
}