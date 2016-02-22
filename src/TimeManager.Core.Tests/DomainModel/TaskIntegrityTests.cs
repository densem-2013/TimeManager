namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using Infocom.TimeManager.Core.DomainModel;

    using Ninject;

    using NUnit.Framework;

    [TestFixture]
    public class TaskIntegrityTests : IntegrityTestBase<Task>
    {
        [Test]
        public void Name_NotNull()
        {
            CheckProperty(e => e.Name).For.NotNull();
        }

        [Test, Ignore("todo")]
        public void CurrentStatus_NotNull()
        {
            CheckProperty(e => e.CurrentStatus).For.NotNull();
        }

        [Test, Ignore("todo")]
        public void Project_NotNull()
        {
            CheckProperty(e => e.Project).For.NotNull();
        }

        protected override Task InitializeSut()
        {
            return MockingKernel.Get<Task>();
        }
    }
}