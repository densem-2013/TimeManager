namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class IncomingRequestModel : BaseModel
    {
        #region Constructors and Destructors

        public IncomingRequestModel()
        {
            this.Customer = new CustomerModel();
        }

        #endregion


        #region Properties

        [DisplayName(@"Наименование")]
        public string Name { get; set; }

        //[UIHint("Numeric")]
        //[RegularExpression(@"[\d]{1,9}([-\\/][\d]{1,4})?", ErrorMessage = "Не допустимое значение")]
        [Required(ErrorMessage = @"Не задано")]
        [DisplayName(@"Номер договора")]
        public string Number { get; set; }

        [DisplayName(@"Дата")]
        public DateTime? SigningDate { get; set; }

        [DisplayName(@"Заказчик")]
        [Required(ErrorMessage = @"Не задано")]
        public CustomerModel Customer { get; set; }

        [DisplayName(@"Тип договора")]
        public long ContractTypeId { get; set; }

        [DisplayName(@"Тип договора")]
        public string ContractTypeName { get; set; }

        #endregion
    }
}