namespace Infocom.TimeManager.WebAccess.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using Telerik.Web.Mvc.Extensions;

    [HandleError]
    public class RequestDetailsController : TimeManagerBaseController
    {
        #region Constants and Fields

        private RoleManagment _userRoleService;

        #endregion

        #region Constructors and Destructors

        public RequestDetailsController()
        {
            this._userRoleService = new RoleManagment(this);
        }

        #endregion

        #region Public Methods

        public ActionResult Index(long requestID=0)
        {
            var model = this.CreateModel(requestID);

            if (model.Request.ID == 0)
            {
                //this.ViewBag.OverviewErrorMessage = "Заявка с данным номером не существует в базе данных";
                this.Session["EditErrorMessage"] = "Заявка с данным номером не существует в базе данных";
                return RedirectToAction("Index", "Requests");
               
            }
            else 
            { 
                this.InitializeViewData(model);
            }
           return View(model);
          
         }

      
        #endregion

        #region Private Methods

        private ViewResult ViewIndex(long requestID)
        {
            var model = this.CreateModel(requestID);
            this.InitializeViewData(model);
            return View("Index", model);
        }

        internal RequestDetailsViewModel CreateModel(long requestID)
        {
            var model = new RequestDetailsViewModel();
            var assembler = new ModelAssembler();
            var persistedRequest = this.PersistenceSession.Get<Request>(requestID);
            if (persistedRequest != null)
            {
                var projects = this.PersistenceSession.QueryOver<Project>().Future();
                var persistedPhases = this.PersistenceSession.QueryOver<Phase>()
                    .Where(p => p.Request.ID == requestID).Future();
                var persistedProjects = persistedPhases
                    .Select(persistedPhase => this.PersistenceSession.QueryOver<Project>()
                        .Where(p => p.Phase.ID == persistedPhase.ID)
                        .Future().FirstOrDefault())
                        .Where(persistedProject => persistedProject != null)
                        .ToList();

                if (persistedRequest == null)
                {
                    this.ModelState.AddModelError(String.Empty,
                        string.Format("Завка с ID='{0}' не найдена.", requestID));
                }
                else
                {
                    var requestModel = assembler.CreateRequestModel(persistedRequest);
                    model.Request = requestModel;
                }

                if (persistedRequest != null)
                {
                    foreach (var phase in model.Request.Phases)
                    {
                        var phaseCount = 0;
                        if (projects != null)
                        {
                            phaseCount = projects.Where(p => p.Phase.ID == phase.ID).Count();
                        }

                        phase.ProcessState = phaseCount > 0 ? "В работе" : RequestProcessState.Необработанная.ToString();
                    }
                }

                if (persistedProjects.Count() > 0)
                {
                    model.Projects = persistedProjects.Select(assembler.CreateProjectModel);
                }


                return model;
            }
            else
            {
                return model;
                              
            }
        }

        private void InitializeViewData(RequestDetailsViewModel model)
        {
            var modelAssembler = new ModelAssembler();
            var requestDetailTypes = this.PersistenceSession.QueryOver<RequestDetailType>().OrderBy(r => r.ID).Asc.Future();
            model.RequestDetailTypes = requestDetailTypes.Select(modelAssembler.CreateRequestDetailTypeModel);
        }

        #endregion
    }
}
