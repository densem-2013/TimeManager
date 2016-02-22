namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using NUnit.Framework;

    using SharpTestsEx;

    public abstract class RepositoryBaseTests<TRepo, TItem> : BasePersistenceTest<TRepo>
        where TItem : DomainObject, new()
        where TRepo : class, IRepository<TItem>, new()
    {
        #region Public Methods

        [Test]
        public virtual void Add_ItemWithNoRef_ShouldBeAdded()
        {
            // arrange
            var itemToAdd = this.CreateSimpleItem();
            var itemsBeforeAdding = this.Sut.GetAll().Count();

            // act
            this.Sut.Add(itemToAdd);
            var itemsAfterAdding = this.Sut.GetAll().Count();

            // assert
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - 1);
        }

        [Test]
        public virtual void Add_ItemWithRef_ShouldBeAdded()
        {
            // arrange
            var itemToAdd = this.CreateComplexItem();
            var itemsBeforeAdding = this.Sut.GetAll().Count();

            // act
            this.Sut.Add(itemToAdd);
            var itemsAfterAdding = this.Sut.GetAll().Count();

            // assert
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - 1);
        }

        [Test,Ignore]
        public virtual void Add_ItemSeveralItems_ShouldBeAdded()
        {
            // arrange
            int itemsQtyToAdd = 5;
            IList<TItem> itemsToAdd = new List<TItem>();
            for (int i = 0; i < itemsQtyToAdd; i++)
            {
                itemsToAdd.Add(this.CreateSimpleItem());
            }
            var itemsBeforeAdding = this.Sut.GetAll().Count();

            // act
            this.Sut.Add(itemsToAdd);
            var itemsAfterAdding = this.Sut.GetAll().Count();

            // assert
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - itemsQtyToAdd);
        }

        [Test]
        [ExpectedException(typeof(ObjectNotExistException))]
        public virtual void Delete_NotExistedId_Exeption()
        {
            // arrange
            var notExistedId = int.MinValue;

            // act
            this.Sut.Delete(notExistedId);
        }

        [Test,Ignore()]
        public virtual void Delete_ExistedId_DeletedObject()
        {
            // arrange
            var itemToTest = this.CreateSimpleItem();

            // act
            Assert.AreEqual(default(int), itemToTest.ID);
            this.Sut.Add(itemToTest);
            Assert.IsTrue(this.Sut.Exists(itemToTest));
            Assert.AreNotEqual(default(int), itemToTest.ID);
            this.Sut.Delete(itemToTest.ID);

            // assert
            Assert.IsFalse(this.Sut.Exists(itemToTest));
        }

        [Test, Ignore()]
        public virtual void Exists_Exists_True()
        {
            // arrange
            var itemToTest = this.CreateSimpleItem();

            // act
            this.Sut.Add(itemToTest);

            // assert
            Assert.IsTrue(this.Sut.Exists(itemToTest));
        }

        [Test,Ignore()]
        public virtual void Exists_NotExists_False()
        {
            // arrange
            var itemToTest = this.CreateSimpleItem();

            // act
            this.Sut.Add(itemToTest);
            Assert.IsTrue(this.Sut.Exists(itemToTest));
            this.Sut.Delete(itemToTest.ID);

            // assert
            Assert.IsFalse(this.Sut.Exists(itemToTest));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void Exists_NullArg_Exception()
        {
            this.Sut.Exists(null);
        }

        [Test,Ignore()]
        public virtual void GetAll_LazyLoading_NoError()
        {
            // arrange
            var itemToTest = this.CreateComplexItem();

            // act
            this.Sut.Add(itemToTest);
            var persistedTask = this.Sut.GetAll().Where(t => t.ID == itemToTest.ID).Single();

            // assert 
            var linkedObject = this.GetLinkedComplexObject(persistedTask);
            Assert.IsNotNull(linkedObject);
        }

        [Test]
        [ExpectedException(typeof(ObjectNotExistException))]
        public virtual void GetById_NotExistedId_Exception()
        {
            // arrange
            var itemID = int.MinValue;

            // act 
            this.Sut.GetById(itemID); // excpetion here
        }

        [Test,Ignore()]
        public virtual void GetById_ExistedId_NotNull()
        {
            // arrange
            var itemToTest = this.CreateSimpleItem();

            // act 
            this.Sut.Add(itemToTest);
            this.Sut.GetById(itemToTest.ID)


                // assert
                .Should().Not.Be.Null();
        }

        [Test,Ignore()]
        public virtual void TryGetById_ExistedId_TrueAndCorrectObject()
        {
            // arrange
            var itemToTest = this.CreateSimpleItem();

            // act
            this.Sut.Add(itemToTest);
            TItem actualItem;
            var isObjectFound = this.Sut.TryGetById(itemToTest.ID, out actualItem);

            // assert
            Assert.IsTrue(isObjectFound, "Object should be found.");
            Assert.IsNotNull(actualItem, "Object should be Retrieved.");
            Assert.AreEqual(itemToTest, actualItem, "Wrong item Retrieved.");
        }

        [Test]
        public virtual void TryGetById_NotExistedId_FalseAndNullObjectRetrieved()
        {
            // arrange
            int itemID = int.MinValue; // not existed id 

            // act
            TItem actualItem;
            var isObjectFound = this.Sut.TryGetById(itemID, out actualItem);

            // assert
            Assert.IsFalse(isObjectFound, "Object should be not found.");
            Assert.IsNull(actualItem, "NUll object should be Retrieved.");
        }

        #endregion

        #region Methods

        protected abstract TItem CreateSimpleItem();

        protected abstract TItem CreateComplexItem();

        protected abstract object GetLinkedComplexObject(TItem source);

        #endregion
    }
}