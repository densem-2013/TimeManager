namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class ProjectsTests : BasePersistenceTest<Project>
    {
        #region Methods

        [Test]
        public void BudgetAllocation_NoMissedAlocations()
        {
            using (var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession())
            using (var tx = session.BeginTransaction())
            {
                var dept1 = new Department
                    {
                        Name = "DIT",
                        ShortName = "DIT",
                        Manager = this.MockingKernel.Get<Employee>()
                    };
                var dept2 = new Department
                {
                    Name = "PKD",
                    ShortName = "PKD",
                    Manager = this.MockingKernel.Get<Employee>()
                };
               // var phase = this.MockingKernel.Get<Phase>();

                var allocation1 = new ProjectBudgetAllocation();
                allocation1.AmountOfHours = 10;
                allocation1.AmountOfMoney = 1000;
               // allocation1.Phase = phase;
                allocation1.Department = dept1;

                var allocation2 = new ProjectBudgetAllocation();
                allocation2.AmountOfHours = 99;
                allocation2.AmountOfMoney = 99;
               // allocation2.Phase = phase;
                allocation2.Department = dept2;

                //project.Budget.BudgetAllocation.Add(allocation1);
                //project.Budget.BudgetAllocation.Add(allocation2);

                session.SaveOrUpdate(allocation1);
                tx.Commit();
            }

            //todo: missed asserts ;)
        }

        [Test]
        public void Contract_SetAndPersist_NoErrors()
        {
            using (var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession())
            using (var tx = session.BeginTransaction())
            {
                var project = this.MockingKernel.Get<Project>();
                //project.Contract.Name = "Name1";
                //project.Contract.SigningDate = null;
                //project.Contract.Number = "N012345";
                //project.Contract.Customer = "Customer";

                session.SaveOrUpdate(project);
                tx.Commit();
            }

            //todo: missed asserts ;)
        }

        [Test]
        public void Ctor_TasksInitialization_AfterInitializationTastsNotNull()
        {
            Assert.IsNotNull(this.Sut.Tasks);
        }

        protected override Project InitializeSut()
        {
            return new Project();
        }

        #endregion
    }
}