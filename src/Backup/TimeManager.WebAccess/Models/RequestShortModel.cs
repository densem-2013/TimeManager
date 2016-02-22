using System;
namespace Infocom.TimeManager.WebAccess.Models
{
    public class RequestShortModel:DynamicBaseModel
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }

    }
}