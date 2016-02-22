namespace Infocom.TimeManager.WebAccess.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Infocom.TimeManager.WebAccess.Models;
    
    public class ContractEditorViewModel
    {
        public IEnumerable<CustomerModel> Customers { get; set; }
        public IEnumerable<ContractTypeModel> ContractTypes { get; set; }
    }
}