namespace Infocom.TimeManager.WebAccess.Models
{
    using System;
    using System.ComponentModel;
    public class CustomerModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Наименование заказчика")]
        public string Name { get; set; }
        
        #endregion
    }
}