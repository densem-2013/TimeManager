using System;

namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class BudgetAllocationModel : BaseModel
    {
        #region Constructors and Destructors

        public BudgetAllocationModel()
        {
            this.Department = new DepartmentModel();
        }

        #endregion

        #region Properties

        [UIHint("Numeric")]
        [RegularExpression(@"[\d]{1,9}([,]\d{1,2})?", ErrorMessage = "Не допустимое значение")]
        [DisplayName(@"Количество грн.")]
        public decimal AmountOfMoney { get; set; }

        [UIHint("Numeric")]
        [RegularExpression(@"[\d]{1,5}", ErrorMessage = "Не допустимое значение")]
        [DisplayName(@"Количество чел./часов")]
        public int AmountOfHours { get; set; }

        public DepartmentModel Department { get; set; }

        [DisplayName(@"Дата окончания работ департаментом")]
        //[Required(ErrorMessage = @"Не задано")]
        public DateTime? DateEndWorkByDepartment { get; set; }

        //public decimal SpentOfMoney { get; set; }

        //public int SpentOfHours { get; set; }

        #endregion
    }
}