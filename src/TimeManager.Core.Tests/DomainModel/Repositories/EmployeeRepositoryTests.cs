namespace Infocom.TimeManager.Core.Tests.DomainModel.Repositories
{
    using System;
    using System.Linq;

    using Infocom.TimeManager.Core.DomainModel;
    using Infocom.TimeManager.Core.DomainModel.Repositories;

    using NHibernate.Exceptions;
    using NHibernate.Validator.Engine;
    using NHibernate.Validator.Exceptions;

    using Ninject;

    using NUnit.Framework;

    using SharpTestsEx;

    using Castle.Core;

    [TestFixture]
    public class EmployeeRepositoryTests : RepositoryBaseTests<EmployeeRepository, Employee>
    {
        #region Public Methods

        [Test]
        public override void Add_ItemWithRef_ShouldBeAdded()
        {
            // arrange
            var itemToAdd = this.CreateComplexItem();
            var itemsBeforeAdding = this.Sut.GetAll().Count();

            // act
            this.Sut.Add(itemToAdd);
            var itemsAfterAdding = this.Sut.GetAll().Count();

            // assert
            Assert.AreEqual(itemsBeforeAdding, itemsAfterAdding - 2);
        }

        [Test]
        public void Add_ManyToMany_ShouldBeAdded()
        {
            // arrange
            var employees = new[] { this.MockingKernel.Get<Employee>(), this.MockingKernel.Get<Employee>() };
            var tasks = new[] { this.MockingKernel.Get<Task>(), this.MockingKernel.Get<Task>() };

            foreach (var employee in employees)
            {
                foreach (var task in tasks)
                {
                    employee.Tasks.Add(task);
                }
            }

            // act
            try
            {
                this.Sut.Add(employees);
            }
            catch (InvalidStateException ex)
            {
                var invalidValues = ex.GetInvalidValues();
                var errorMessage = ex.Message;
                foreach (var invalidValue in invalidValues)
                {
                    errorMessage += "\n" + invalidValue.Message;
                }

                throw new ApplicationException(errorMessage);
            }

            // assert
            foreach (var employee in employees)
            {
                var actualEmployee = Sut.GetById(employee.ID);
                Assert.AreEqual(tasks.Count(), actualEmployee.Tasks.Count());
                foreach (var task in employee.Tasks)
                {
                    Assert.AreEqual(employees.Count(), task.AssignedEmployees.Count());
                }
            }
        }

        [Test]
        public void Add_TwiceWithSameLogin_UniqueKeyViolation()
        {
            // arrange
            var itemToAdd1 = this.CreateComplexItem();
            var itemToAdd2 = this.CreateComplexItem();
            itemToAdd2.Login = itemToAdd1.Login;

            // act
            try
            {
                this.Sut.Add(itemToAdd1); // exception here
                this.Sut.Add(itemToAdd2); // or here
            }
            catch (GenericADOException ex)
            {
                ex.Should().Not.Be.Null();
                ex.InnerException.Message.Should().Contain("Violation of UNIQUE KEY");
                return;
            }

            Assert.Fail("Violation of UNIQUE KEY expected");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByLogin_NullArg_Exception()
        {
            // arrange
            string login = null;

            // act
            this.Sut.GetByLogin(login);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByLogin_EmptyArg_Exception()
        {
            // arrange 
            string login = string.Empty;

            // act
            this.Sut.GetByLogin(login);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByLogin_WhiteSpacesArg_Exception()
        {
            // arrange
            string login = "   ";

            // act
            this.Sut.GetByLogin(login);
        }

        [Test]
        public void GetByLogin_ExistedLogin_CorrectObject()
        {
            // arrange
            var employee = new Employee { FirstName = "Alexey", PatronymicName = "Petrovich", LastName = "Mukas", Login = Guid.NewGuid().ToString() };
            this.Sut.Add(employee);

            // act
            var requestedLogin = this.Sut.GetByLogin(employee.Login);

            // assert
            Assert.AreEqual(employee, requestedLogin);
        }

        #endregion

        #region Methods

        protected override Employee CreateSimpleItem()
        {
            return this.MockingKernel.Get<Employee>();
        }

        protected override Employee CreateComplexItem()
        {
            var result = this.CreateSimpleItem();
            result.Manager = this.CreateSimpleItem();

            return result;
        }

        protected override object GetLinkedComplexObject(Employee source)
        {
            return source.Manager;
        }

        protected override EmployeeRepository InitializeSut()
        {
            return new EmployeeRepository();
        }

        #endregion
    }
}