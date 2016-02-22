namespace Infocom.TimeManager.WebAccess.Tests
{
    using Moq;

    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;

    using NUnit.Framework;

    public abstract class BaseTest<T> : BaseTest
        where T : class
    {
        #region Properties

        protected Mock<T> SutMock { get; private set; }

        protected T Sut { get; private set; }

        #endregion

        #region Methods

        public override void SetUp()
        {
            base.SetUp();

            this.Sut = this.InitializeSut();
        }

        protected abstract T InitializeSut();

        #endregion
    }

    public abstract class BaseMockTest<T> : BaseTest
    where T : class
    {
        #region Properties

        protected Mock<T> SutMock { get; private set; }

        protected T Sut { get; private set; }

        #endregion

        #region Methods

        public override void SetUp()
        {
            base.SetUp();

            this.SutMock = this.InitializeSutMock();
            this.Sut = this.SutMock.Object;
        }

        protected abstract Mock<T> InitializeSutMock();

        #endregion
    }


    public abstract class BaseTest
    {
        #region Constructors and Destructors

        public BaseTest()
        {
            this.MockingKernel = new MoqMockingKernel();
        }

        #endregion

        #region Properties

        protected MockingKernel MockingKernel { get; private set; }

        #endregion

        #region Methods

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.MockInit();
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        protected virtual void MockInit()
        {
        }

        #endregion
    }
}