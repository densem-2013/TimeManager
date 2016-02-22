namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class ProjectIntegrityTests : IntegrityTestBase<Project>
    {

        [Test]
        public void Name_NotNull()
        {
            CheckProperty(e => e.Name).For.NotNull();
        }

        [Test]
        public void Code_NotNull()
        {
            CheckProperty(e => e.Code).For.NotNull();
        }

        [Test, Ignore("todo")]
        public void CurrentStatus_NotNull()
        {
            CheckProperty(e => e.CurrentStatus).For.NotNull();
        }

        protected override Project InitializeSut()
        {
            return MockingKernel.Get<Project>();
        }
    }
}