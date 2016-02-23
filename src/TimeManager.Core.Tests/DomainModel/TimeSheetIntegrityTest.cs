//namespace Infocom.TimeManager.Core.Tests.DomainModel
//{
//    using Infocom.TimeManager.Core.DomainModel;

//    using Ninject;

//    using NUnit.Framework;

//    [TestFixture]
//    public class TimeSheetIntegrityTest : IntegrityTestBase<TimeSheet>
//    {
//        [Test]
//        public void Employee_NotNull()
//        {
//            CheckProperty(e => e.Employee).For.NotNull();
//        }

//        [Test]
//        public void WorkWeek_NotNull()
//        {
//            CheckProperty(e => e.WorkWeek).For.NotNull();
//        }

//        [Test]
//        public void Year_NotNull()
//        {
//            CheckProperty(e => e.Year).For.NotNull();
//        }

//        protected override TimeSheet InitializeSut()
//        {
//            return MockingKernel.Get<TimeSheet>();
//        }
//    }
//}
