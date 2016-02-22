using System;
namespace Infocom.TimeManager.WebAccess.Models
{
    using System.ComponentModel;

    public class RequestTypeModel : BaseModel
    {
        [DisplayName(@"Тип заявки")]
        public string Name { get; set; }
    }
}
