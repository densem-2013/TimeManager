namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class RequestsViewModel : BaseViewModel
    {
        public RequestsViewModel()
        {
            this.Requests = new List<RequestOverviewModel>();
        }

        public IEnumerable<RequestOverviewModel> Requests { get; set; }

        }
}