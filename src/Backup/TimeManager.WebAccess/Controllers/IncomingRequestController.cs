namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Filters;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using Telerik.Web.Mvc.Extensions;
    using NHibernate;
    using NHibernate.Criterion;
    
    [HandleError]
    public partial class IncomingRequestController : TimeManagerBaseController
    {
        private readonly string _contractsFilterKey = "IncomingRequestFilter";

        #region Public Methods

        public virtual ActionResult Index()
        {
            var model = this.CreateModel();
            this.InitializeViewData();

            return View(model);
        }

        internal IncomingRequestViewModel CreateModel()
        {
            var filter = new ContractsFilter();

            if (Session[this._contractsFilterKey] != null)
            {
                filter = (ContractsFilter)Session[this._contractsFilterKey];
            }

            var persistedContracts = GetPersistedContracts(filter);

            var modelAssembler = new ModelAssembler();
            var model = new IncomingRequestViewModel();
            var previewContract = persistedContracts.Select(modelAssembler.CreateContractModel);

            model.IncomingRequests = previewContract.Where(c=>c.ContractTypeId>2).OrderByDescending(r => r.Number);
                //GetRequestWithProcessState(previewContract, filter).OrderByDescending(r => r.Number);
            return model;
        }

        [HttpPost]
        public virtual ActionResult GetContractsName(long customerID)
        {
            var modelAssembler = new ModelAssembler();
            IEnumerable<ContractModel> result = new List<ContractModel>();
            result = PersistenceSession.QueryOver<Contract>().Where(c => c.Customer.ID == customerID).Future().Select(modelAssembler.CreateContractModel);
            return new JsonResult { Data = new SelectList(result, "ID", "Name") };
        }

        [HttpPost]
        public virtual ActionResult Index(string filteringBySearchWord)
        {
            var filter = new ContractsFilter();
            filter.FilteringBySearchWord = filteringBySearchWord;
            Session[this._contractsFilterKey] = filter;

            var model = this.CreateModel();
            //this.InitializeViewData();
            var view = View(model);
            return view;
        }

        private void InitializeViewData()
        {
            var modelAssembler = new ModelAssembler();
            
            var customers = this.PersistenceSession.QueryOver<Customer>().Future();
            var contractTypes = this.PersistenceSession.QueryOver<ContractType>().Future();

            var contractEditorModel = new ContractEditorViewModel 
            { 
                Customers = customers.Select(modelAssembler.CreateCustomerModel),
                ContractTypes = contractTypes.Where(ct=>ct.ID>2).Select(modelAssembler.CreateContractTypeModel)
            };
            ViewBag.ContractEditorViewModel = contractEditorModel;

            if (Session["EditErrorMessage"] != null)
            {
                this.ViewBag.OverviewErrorMessage = Session["EditErrorMessage"].ToString();
                Session["EditErrorMessage"] = null;
            }
        }


        private IEnumerable<Contract> GetPersistedContracts(ContractsFilter filter)
        {
            var result = new List<Contract>();
            var persistedContracts = this.PersistenceSession.QueryOver<Contract>().Future();
            IEnumerable<Contract> persistedContracts2 = persistedContracts;
            if (!string.IsNullOrEmpty(filter.FilteringBySearchWord))
            {
               var persistedContracts1 =
                   persistedContracts.Where(c => c.Name != null).ToList();
               persistedContracts2 = persistedContracts1.Where(c =>
                       c.Customer.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       ||
                       c.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       );
            }

            result.AddRange(persistedContracts2);
            return result;
        }

        private ActionResult RedirectToIndex()
        {
            return this.RedirectToAction("Index");
        }

        private ViewResult ViewIndex()
        {
            this.InitializeViewData();
            return View("Index");
        }

        [HttpPost]
        public virtual ActionResult IncomingRequestUpdate(IncomingRequestModel incomingRequestModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var contractToUpdate = PersistenceSession.Get<Contract>(incomingRequestModel.ID);
                        contractToUpdate.Name = incomingRequestModel.Name;
                        contractToUpdate.SigningDate = incomingRequestModel.SigningDate;
                        var contractType = PersistenceSession.Get<ContractType>(incomingRequestModel.ContractTypeId);
                        
                        var customer = PersistenceSession.QueryOver<Customer>()
                           .Where(c => c.Name == incomingRequestModel.Customer.Name).SingleOrDefault();

                        if (customer != null)
                        {
                            contractToUpdate.Customer = customer;
                        }
                        else
                        {
                            var newCustomer = new Customer { Name = incomingRequestModel.Customer.Name };
                            this.PersistenceSession.SaveOrUpdate(newCustomer);

                            customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == incomingRequestModel.Customer.Name).SingleOrDefault();
                            contractToUpdate.Customer = customer;
                        }
                        switch (incomingRequestModel.ContractTypeId)
                        {
                            case 3: contractToUpdate.Name = "Разработка ТП";
                                break;
                            case 4: contractToUpdate.Name = "Административные работы";
                                break;
                            default:
                                break;
                        }
                        contractToUpdate.Number = CreateRequestNumber(incomingRequestModel.Number, incomingRequestModel.ContractTypeId);
                        contractToUpdate.ContractType = contractType;

                        this.PersistenceSession.SaveOrUpdate(contractToUpdate);
                        tx.Commit();

                    }
                    catch (Exception ex)
                    {
                        this.CreateOrederEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", ex.Message));
                    }
                    return RedirectToIndex();
                }
            }
            return RedirectToIndex();
        }

        [HttpPost]
        public virtual ActionResult IncomingRequestInsert(IncomingRequestModel incomingRequestModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var modelAssembler = new ModelAssembler();

                        var newContract = new Contract();
                        newContract = modelAssembler.CreateContractFromRequest(incomingRequestModel);
                        var contractType = PersistenceSession.Get<ContractType>(incomingRequestModel.ContractTypeId);
                        var customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == incomingRequestModel.Customer.Name.Trim()).SingleOrDefault();

                        if (customer == null)
                        {
                            var newCustomer = new Customer { Name = incomingRequestModel.Customer.Name };
                            this.PersistenceSession.SaveOrUpdate(newCustomer);

                            customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == incomingRequestModel.Customer.Name).SingleOrDefault();
                        }

                        var IsExist = PersistenceSession.QueryOver<Contract>()
                            .Where(c =>( c.Name == incomingRequestModel.Name 
                                        && c.Customer.ID==incomingRequestModel.Customer.ID)||
                                        (c.Number == incomingRequestModel.Number &&
                                        c.SigningDate == incomingRequestModel.SigningDate))
                                  .Future().Any();
                       
                        if (IsExist)
                        {
                            this.CreateOrederEditFormErrorMessage("Создание администартивной заявки не возможно, т.к. администартивная заявку уже существует в списке запросов.");
                        }
                        switch (incomingRequestModel.ContractTypeId)
                        {
                            case 3: newContract.Name = "Разработка ТП";
                                break;
                            case 4: newContract.Name = "Административные работы";
                                break;
                            default:
                                break;
                        }
                        newContract.Number = CreateRequestNumber(incomingRequestModel.Number, incomingRequestModel.ContractTypeId);
                        newContract.Customer = customer;
                        newContract.ContractType = contractType;
                        this.PersistenceSession.SaveOrUpdate(newContract);
                        tx.Commit();
                       
                    }
                    catch (Exception ex)
                    {
                        this.CreateOrederEditFormErrorMessage(string.Format("Загрузка данных прервана из-за ошибки: {0}", ex.Message));
                    }
                    return RedirectToIndex();
                }
            }

            return this.ViewIndex();
        }

        private string CreateRequestNumber(string contractNumber, long contractTypeId)
        {
            var result = string.Empty;
            var contractType = this.PersistenceSession.Get<ContractType>(contractTypeId);
            result = string.Format("{0} № {1}",contractType.ShortName ,contractNumber);
            return result;
        }

        private void CreateOrederEditFormErrorMessage(string message)
        {
            this.Session["EditErrorMessage"] = message;
        }

        [HttpPost]
        public virtual ActionResult IncomingRequestDelete(long id)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    
                    var contractToDelete = this.PersistenceSession.Get<Contract>(id);
                    var IsCanBeDelete = !this.PersistenceSession.QueryOver<Request>().Where(r=>r.Contract.ID == id).Future().Any();
                    if (IsCanBeDelete)
                    {
                        this.PersistenceSession.Delete(contractToDelete);
                        tx.Commit();
                    }
                    else
                    {
                        this.CreateOrederEditFormErrorMessage(string.Format("Удаление администартивная заявка не возможно. По администартивной заявке уже существует заявка"));
                    }
                }

                return RedirectToIndex();
            }

            return this.ViewIndex();
        }

        #endregion
    }
}