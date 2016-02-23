namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;

    public class ContractTypeModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Наименование")]
        public string Name { get; set; }

        #endregion
    }
}