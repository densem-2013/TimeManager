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
    public partial class ContractsController : TimeManagerBaseController
    {
        private readonly string _contractsFilterKey = "ContractsFilter";

        #region Public Methods

        public virtual ActionResult Index()
        {
            var model = this.CreateModel();
            this.InitializeViewData();

            return View(model);
        }

        internal ContractViewModel CreateModel()
        {
            var filter = new ContractsFilter();

            if (Session[this._contractsFilterKey] != null)
            {
                filter = (ContractsFilter)Session[this._contractsFilterKey];
            }

            var persistedContracts = GetPersistedContracts(filter);

            var modelAssembler = new ModelAssembler();
            var model = new ContractViewModel();
            var previewContract = persistedContracts.Select(modelAssembler.CreateContractModel);

            model.Contracts = previewContract.Where(ct => ct.ContractTypeId < 3).OrderByDescending(r => r.SigningDate);
                //GetRequestWithProcessState(previewContract, filter).OrderByDescending(r => r.Number);
            return model;
        }

        [HttpPost]
        public virtual ActionResult GetContractsName(long customerID=0)
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
                ContractTypes = contractTypes.Where(ct => ct.ID < 3).Select(modelAssembler.CreateContractTypeModel)
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
        public virtual ActionResult ContractUpdate(ContractModel contractModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var contractToUpdate = PersistenceSession.Get<Contract>(contractModel.ID);
                        contractToUpdate.Name = contractModel.Name;
                        contractToUpdate.SigningDate = contractModel.SigningDate;
                        var contractType = PersistenceSession.Get<ContractType>(contractModel.ContractTypeId);
                        
                        var customer = PersistenceSession.QueryOver<Customer>()
                           .Where(c => c.Name == contractModel.Customer.Name).SingleOrDefault();

                        if (customer != null)
                        {
                            contractToUpdate.Customer = customer;
                        }
                        else
                        {
                            var newCustomer = new Customer { Name = contractModel.Customer.Name };
                            this.PersistenceSession.SaveOrUpdate(newCustomer);

                            customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == contractModel.Customer.Name).SingleOrDefault();
                            contractToUpdate.Customer = customer;
                        }

                        contractToUpdate.Number = CreateRequestNumber(contractModel.Number, contractModel.ContractTypeId);
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
        public virtual ActionResult ContractInsert(ContractModel contractModel)
        {
            if (this.ModelState.IsValid)
            {
                using (var tx = this.PersistenceSession.BeginTransaction())
                {
                    try
                    {
                        var modelAssembler = new ModelAssembler();

                        var newContract = new Contract();
                        newContract = modelAssembler.CreateContract(contractModel);
                        var contractType = PersistenceSession.Get<ContractType>(contractModel.ContractTypeId);
                        var customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == contractModel.Customer.Name.Trim()).SingleOrDefault();

                        if (customer == null)
                        {
                            var newCustomer = new Customer { Name = contractModel.Customer.Name };
                            this.PersistenceSession.SaveOrUpdate(newCustomer);

                            customer = PersistenceSession.QueryOver<Customer>()
                            .Where(c => c.Name == contractModel.Customer.Name).SingleOrDefault();
                        }

                        var IsExist = PersistenceSession.QueryOver<Contract>()
                            .Where(c =>( c.Name == contractModel.Name 
                                        && c.Customer.ID==contractModel.Customer.ID)||
                                        (c.Number == contractModel.Number &&
                                        c.SigningDate == contractModel.SigningDate))
                                  .Future().Any();
                       
                        if (IsExist)
                        {
                            this.CreateOrederEditFormErrorMessage("Создание договора не возможно, т.к. договор уже существует в списке договоров.");
                        }
                        newContract.Number = CreateRequestNumber(contractModel.Number, contractModel.ContractTypeId);
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
        public virtual ActionResult ContractDelete(long id)
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
                        this.CreateOrederEditFormErrorMessage(string.Format("Удаление договора не возможно. По договору уже существует заявка"));
                    }
                }

                return RedirectToIndex();
            }

            return this.ViewIndex();
        }

        #endregion
    }
}