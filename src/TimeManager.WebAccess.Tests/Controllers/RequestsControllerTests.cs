namespace Infocom.TimeManager.WebAccess.Tests.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.WebAccess.Controllers;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Infocom.TimeManager.WebAccess.Models;
    using Infocom.TimeManager.WebAccess.ViewModels;

    using Moq;

    using NHibernate;

    using NUnit.Framework;

    using SharpTestsEx;
    using Microsoft.Practices.ServiceLocation;
    using System.Collections.Generic;
    using Infocom.TimeManager.WebAccess.Filters;

    [TestFixture]
    public class RequestsControllerTests : BasePersistenceMockTest<RequestsController>
    {
        #region Methods

        public override void SetUp()
        {
            base.SetUp();
            this.InitializeDBSchema();
        }

        protected override Mock<RequestsController> InitializeSutMock()
        {
            return new Mock<RequestsController>() { CallBase = true };
        }

        [Test, Ignore("check it")]
        public void Index_ModelCheck_NoErrors()
        {

            // act
            var result = this.GetViewResult();

            // assert
            result.Should().Not.Be.Null();
            result.Model.Should().Not.Be.Null();
        }

        private ViewResult GetViewResult()
        {
            return this.Sut.Index() as ViewResult;
        }

        #endregion

        [Test]
        public void RequestInsert_InsertRequestCorrectly_CreateNewRequestAndInsert()
        {
            // Arrange
            var modelAssembler = new ModelAssembler();

            var department = Controllers.Model.CreateDepartment();
            var requestType = Controllers.Model.CreateRequestType();
            var requestTypeModel = modelAssembler.CreateRequestTypeModel(requestType);
            var departmentModel = modelAssembler.CreateDepartmentModel(department);
            var user = Controllers.Model.CreateUser(department);
            var request = new RequestModel();
            var allocaction = new BudgetAllocationModel
                {
                    AmountOfHours = 100,
                    AmountOfMoney = 1200,
                    Department = departmentModel
                };
            
            var phase = new PhaseModel();
            phase.Name = "TestPhase";
            phase.Budget.BudgetAllocations.Add(allocaction);
            phase.DeadLine = new DateTime(2011, 1, 1);
            //request.Contract = new ContractModel{ Name = "Test", Number = "0032",SigningDate = new DateTime(2011,1,1)};
            request.Date = new DateTime(2011, 1, 1);
            request.Number = "№454545";
            request.Type = requestTypeModel;
            request.ProjectManagerId = user.ID;
            request.ProjectManagerShortName=user.ShortName; //modelAssembler.CreateEmployeeModel(user);
            request.Phases.Add(phase);
            
            this._SetupUserFeature(user);
            // Act
            var actualTimeSheet = Sut.RequestInsert(request);

        }

        [Test]
        public void CreateModel_CorrectViewModel()
        {
            // Arrange
            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var request = Controllers.Model.CreateRequest();
            this._SetupUserFeature(user);

            // Act
            var actualViewModel = Sut.CreateModel();

            // Assert
            Assert.IsNotNull(actualViewModel, "Actual shouldn't be null.");
            Assert.IsNotEmpty(actualViewModel.Requests.ToArray(), "Some tasks should be retrieved .");
            Assert.AreEqual(1, actualViewModel.Requests.Count(), "Expected and actual items count should be the same.");
            //Assert.AreEqual(request.Number, actualViewModel.Requests.ToList()[0].Number, "Expected and actual Number should be the same.");
            //Assert.AreEqual(request.Phases.Count, actualViewModel.Requests.ToList()[0].Phases.Count, "Expected and actual Phases count should be the same.");
            
            //foreach (var phase in actualViewModel.Requests.ToList()[0].Phases)
            //{
            //    Assert.IsTrue(
            //        phase.Budget.BudgetAllocations
            //                            .Where(ba => ba.Department.ShortName 
            //                                == department.ShortName).Count() == 1,
            //        "Not expected Actual task found.");
            //}

        }

        [Test]
        public void HowFastItWork()
        {
            var user = new Employee();
             using (var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession())
             {
                 user = session.Get<Employee>((long)65538);
            
           
            this._SetupUserFeature(user);
            var countOfCicle = 1;
            var startDate = DateTime.Now;
            for (int i = 0; i < countOfCicle; i++)
            {
                //var test = Sut.GetCustomer("bu");
                var model= Sut.CreateModel();
               // Sut.InitializeViewData();
               // Sut.Index();
            }
            var finishDate = DateTime.Now;
            var spent = string.Format("{0:mm:ss.fff} или {1}", new DateTime((finishDate.Ticks - startDate.Ticks) / 10), (finishDate.Ticks - startDate.Ticks) / 10);
            var o = "";
            }
        }

        [Test]
        public void ForTests()
        {
            var user = new Employee();
            using (var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession())
            {
                user = session.Get<Employee>((long)65538);
                var modelAssembler = new ModelAssembler();

                this._SetupUserFeature(user);
                var startDate = DateTime.Now;

                var filter = new RequestsFilter();
                var persistedRequests = GetPersistedRequests(filter);
                var previewRequest = persistedRequests.OrderByDescending(r => r.ID).OrderBy(r => r.IsValidated).Select(modelAssembler.CreateRequestModel);
                var requestWithProcessState = previewRequest;// GetRequestWithProcessState(previewRequest, filter);
                var assignedRequests = requestWithProcessState.Where(r => r.ProjectManagerId == user.ID);
                var notAssignedRequests = requestWithProcessState.Where(r => r.ProjectManagerId != user.ID);
                    
               
                var finishDate = DateTime.Now;
                var spent = string.Format("{0:mm:ss.fff} или {1}", new DateTime(finishDate.Ticks - startDate.Ticks), finishDate.Ticks - startDate.Ticks);
                var o = "";
            }
        }

        [Test]
        public void UpdateModel_CorrectViewModel()
        {
            // Arrange
            var modelAssembler = new ModelAssembler();

            var department = Controllers.Model.CreateDepartment();
            var user = Controllers.Model.CreateUser(department);
            var request = Controllers.Model.CreateRequest();
            var requestModel = modelAssembler.CreateRequestModel(request);
            this._SetupUserFeature(user);

            // Act
            var actualViewModel = Sut.RequestUpdate(requestModel);
        }


        private void _SetupUserFeature(Employee user)
        {
            var userFeature = new Mock<IFutureValue<Employee>>();
            userFeature.Setup(uf => uf.Value).Returns(user);
            SutMock.Setup(c => c.CurrentEmployee).Returns(userFeature.Object);
        }

        private IEnumerable<RequestModel> GetRequestWithProcessState(IEnumerable<RequestModel> requests, RequestsFilter filter)
        {
            var projects = Sut.PersistenceSession.QueryOver<Project>().Future();
            var result = new List<RequestModel>();
            foreach (var request in requests)
            {
                var requestWithProcessState = request;

                bool phasesCount = false;

                foreach (var phase in request.Phases)
                {
                    phasesCount = projects.Where(p => p.Phase.ID == phase.ID).Any();
                }

                if (phasesCount)
                {
                    requestWithProcessState.ProcessState = RequestProcessState.Вработе;
                }
                else
                {
                    requestWithProcessState.ProcessState = RequestProcessState.Необработанная;
                }

                result.Add(requestWithProcessState);
            }

            // if the user use the filter, the '2' means that the filter is not applied 
            if (filter.FilteringByStatusID > 0)
            {
                result = result
                    .Where(r => (int)r.ProcessState == filter.FilteringByStatusID).ToList();
            }

            return result;
        }
        private IEnumerable<Request> GetPersistedRequests(RequestsFilter filter)
        {
            var result = new List<Request>();
            var persistedRequests = this.Sut.PersistenceSession.QueryOver<Request>().Future();
            // if the user use the filter, the '0' means that the filter is not applied 
            // '4' means "New and Opened" projects
            if (filter.FilteringByRequestTypeID != 0)
            {
                persistedRequests =
                    persistedRequests.Where(p => p.Type.ID == filter.FilteringByRequestTypeID);
            }

            if (!string.IsNullOrEmpty(filter.FilteringBySearchWord))
            {
                persistedRequests =
                   persistedRequests.Where(r =>
                       r.Number.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       ||
                       r.Contract.Customer.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                           //||
                           //r.Contract.Name.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       ||
                       r.ProjectManager.ShortName.ToLower().IndexOf(filter.FilteringBySearchWord.ToLower()) >= 0
                       );
            }

            result.AddRange(persistedRequests);
            return result;
        }

    }
}
