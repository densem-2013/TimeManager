namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class IncomingRequestViewModel : BaseViewModel
    {
        public IncomingRequestViewModel()
        {
            this.IncomingRequests = new List<ContractModel>();
        }

        public IEnumerable<ContractModel> IncomingRequests { get; set; }
    }
}