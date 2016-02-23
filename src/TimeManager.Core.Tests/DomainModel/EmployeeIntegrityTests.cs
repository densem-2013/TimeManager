namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class EmployeeIntegrityTests : IntegrityTestBase<Employee>
    {
        [Test]
        public void FirstName_NotNull()
        {
            CheckProperty(e => e.FirstName).For.NotNull();
        }

        [Test]
        public void LastName_NotNull()
        {
            CheckProperty(e => e.LastName).For.NotNull();
        }

        [Test]
        public void Login_NotNull()
        {
            CheckProperty(e => e.Login).For.NotNull();
        }
        
        [Test]
        public void PatronymicName_NotNull()
        {
            CheckProperty(e => e.PatronymicName).For.NotNull();
        }

        protected override Employee InitializeSut()
        {
            return MockingKernel.Get<Employee>();
        }
    }
}