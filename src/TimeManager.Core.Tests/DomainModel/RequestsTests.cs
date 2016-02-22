namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Microsoft.Practices.ServiceLocation;

    using NHibernate;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class RequestsTests : BasePersistenceTest<Request>
    {
        [Test]
        public void CreateRequest_NoMissedParameters()
        {
            
            using (var session = ServiceLocator.Current.GetInstance<ISessionFactory>().GetCurrentSession())
                using (var tx = session.BeginTransaction())
                {

                    var dept1 = new Department
                        { Name = "DIT", ShortName = "DIT", Manager = this.MockingKernel.Get<Employee>() };

                    var dept2 = new Department
                        { Name = "PKD", ShortName = "PKD", Manager = this.MockingKernel.Get<Employee>() };

                    session.SaveOrUpdate(dept1);
                    session.SaveOrUpdate(dept2);

                    var phase = this.MockingKernel.Get<Phase>();
                    phase.DeadLine = new DateTime(2011, 1, 1);
                    phase.Name = "TestPhase";

                    var allocation1 = new ProjectBudgetAllocation();
                    allocation1.AmountOfHours = 10;
                    allocation1.AmountOfMoney = 1000;
                    allocation1.Department = dept1;

                    var allocation2 = new ProjectBudgetAllocation();
                    allocation2.AmountOfHours = 99;
                    allocation2.AmountOfMoney = 99;
                    allocation2.Department = dept2;

                    //    session.SaveOrUpdate(allocation1);
                    //    session.SaveOrUpdate(allocation2);

                    phase.Budget.BudgetAllocations.Add(allocation1);
                    phase.Budget.BudgetAllocations.Add(allocation2);

                    session.SaveOrUpdate(phase);

                    var request = new Request();
                    request.Contract = new Contract
                        {
                            Customer = new Customer {Name = "TestCustomer"},
                            Name = "TestContract",
                            Number = "12",
                            SigningDate = new DateTime(2011, 1, 1)
                        };
                    request.Date = new DateTime(2011, 5, 5);
                    request.Number = "123";
                    request.Phases.Add(phase);
                    request.Type = new RequestType { Description = "Project", Name = "Project" };
                    session.SaveOrUpdate(request);
                    tx.Commit();
                
            
        }
    }

        protected override Request InitializeSut()
        {
            return new Request();
        }
    }
}
