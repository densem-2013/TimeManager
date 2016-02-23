namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using NUnit.Framework;

    [TestFixture]
    public class StatusRepositoryTests : RepositoryBaseTests<StatusRepository, Status>
    {
        #region Public Methods

        public override void Add_ItemWithRef_ShouldBeAdded()
        {
            throw new InvalidOperationException();
        }

        public override void GetAll_LazyLoading_NoError()
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region Methods

        protected override Status CreateSimpleItem()
        {
            return new Status { Name = "New" };
        }

        protected override Status CreateComplexItem()
        {
            throw new InvalidOperationException();
        }

        protected override object GetLinkedComplexObject(Status source)
        {
            throw new InvalidOperationException();
        }

        protected override StatusRepository InitializeSut()
        {
            return new StatusRepository();
        }

        #endregion
    }
}