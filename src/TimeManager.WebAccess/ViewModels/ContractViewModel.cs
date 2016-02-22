namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System.Collections.Generic;

    using Infocom.TimeManager.WebAccess.Models;

    public class ContractViewModel : BaseViewModel
    {
        public ContractViewModel()
        {
            this.Contracts = new List<ContractModel>();
        }

        public IEnumerable<ContractModel> Contracts { get; set; }
    }
}