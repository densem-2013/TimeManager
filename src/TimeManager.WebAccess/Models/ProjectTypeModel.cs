namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;

    public class ProjectTypeModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Наименование")]
        public string Name { get; set; }

        #endregion
    }
}