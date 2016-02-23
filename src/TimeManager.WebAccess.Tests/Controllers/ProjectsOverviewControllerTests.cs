namespace Infocom.TimeManager.WebAccess.Tests.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Infocom.TimeManager.WebAccess.Controllers;
    using Infocom.TimeManager.WebAccess.ViewModels;
    using NUnit.Framework;
    using NHibernate.Linq;

    using NUnit.Framework;

    using SharpTestsEx;
    using Moq;
    using NHibernate;
    using Infocom.TimeManager.WebAccess.Filters;
    using System;
    using Infocom.TimeManager.WebAccess.Infrastructure;
    using Microsoft.Practices.ServiceLocation;


    using Infocom.TimeManager.Core.DomainModel;
    

    [TestFixture]
    public class ProjectsOverviewControllerTests : BasePersistenceMockTest<ProjectsOverviewController>
    {
        #region Methods

        public override void SetUp()
        {
            base.SetUp();

            this.InitializeDBSchema();
        }

        protected override Mock<ProjectsOverviewController> InitializeSutMock()
        {
            return new Mock<ProjectsOverviewController>() { CallBase = true };
        }

        private ViewResult GetViewResult()
        {
            return this.Sut.Index() as ViewResult;
        }


        [Test]
        public void Index_ModelCheck_NoErrors()
        {
            // act
            var result = this.Sut.Index() as ViewResult;

            // assert
            result.Should().Not.Be.Null();
            result.Model.Should().Not.Be.Null();
            result.Model.Should().Be.InstanceOf<ProjectsOverviewViewModel>();
        }

        [Test]
        public void Index_AllEmployeesInViewBag_NoErrors()
        {
            // act
            var viewResult = this.Sut.Index() as ViewResult;
            Assert.IsNotNull(viewResult);
            var viewResultModel = viewResult.Model as ProjectsOverviewViewModel;

            // assert;
            Assert.IsNotNull(viewResultModel);
            Assert.AreEqual(viewResultModel.AllEmployees, Sut.ViewBag.AllEmployees);
            Assert.AreEqual(viewResultModel.AllEmployees, Sut.ViewData["AllEmployees"]);
        }

        [Test]
        public void Index_NoMissedProjects_NoErrors()
        {
            // act
            var result = this.Sut.Index() as ViewResult;
            var resultModel = result.Model.As<ProjectsOverviewViewModel>();

            // assert
            resultModel.Should().Not.Be.Null();
            resultModel.Projects.Count().Should().Be.EqualTo(this.Model.Projects.Count());
        }

        [Test]
        public void Index_NoMissedProperties_NoErrors()
        {
            // act
            var result = this.Sut.Index() as ViewResult;
            var resultModel = result.Model.As<ProjectsOverviewViewModel>();
            var modelProjectOveriview = resultModel.Projects.First();

            // assert
            resultModel.Should().Not.Be.Null();
            var persistedProject = this.Model.Projects.Where(p => p.ID == modelProjectOveriview.ID).Single();
            modelProjectOveriview.Name.Should().Be.EqualTo(persistedProject.Name);
            //modelProjectOveriview.ProjectManager.ShortName.Should().Be.EqualTo(persistedProject.ProjectManager.ShortName);
            //modelProjectOveriview.ProjectManager.ID.Should().Be.EqualTo(persistedProject.ProjectManager.ID);
            //modelProjectOveriview.ProjectOrder.Should().Be.EqualTo(persistedProject.ProjectOrder);
            //modelProjectOveriview.Rate.Should().Be.EqualTo(persistedProject.Rate);
            //modelProjectOveriview.Budget.Should().Be.EqualTo(persistedProject.Budget);
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

                //Sut.Index();
               // var model = Sut.CreateModel();
                //Sut.InitializeViewData();

                var finishDate = DateTime.Now;
                var spent = string.Format("{0:mm:ss.fff} или {1}", new DateTime(finishDate.Ticks - startDate.Ticks), finishDate.Ticks - startDate.Ticks);
                var o = "";
            }

        }



        private void _SetupUserFeature(Employee user)
        {
            var userFeature = new Mock<IFutureValue<Employee>>();
            userFeature.Setup(uf => uf.Value).Returns(user);
            SutMock.Setup(c => c.CurrentEmployee).Returns(userFeature.Object);
        }

       

        #endregion
    }
}