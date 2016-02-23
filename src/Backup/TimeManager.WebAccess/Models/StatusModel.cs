namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;

    using Infocom.TimeManager.Core.DomainModel;

    public class StatusModel : BaseModel
    {
        #region Properties

        [DisplayName(@"Статус проекта:")]
        public string Name { get; set; }

        public DomailEntityType ApplicableTo { get; set; }

        #endregion
    }
}