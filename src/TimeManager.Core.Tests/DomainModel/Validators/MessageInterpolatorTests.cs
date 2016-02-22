namespace Infocom.TimeManager.Core.Tests.DomainModel.Validators
{
    using System;
    using System.Linq.Expressions;

    using Infocom.TimeManager.Core.DomainModel.Validators;

    using Moq;

    using NHibernate.Linq;
    using NHibernate.Validator.Engine;

    using NUnit.Framework;

    using SharpTestsEx;

    [TestFixture]
    public class MessageInterpolatorTests : BaseTest<MessageInterpolator>
    {
        #region Constants and Fields

        private Mock<MessageInterpolator> _mock;

        private string _template;

        private string _templateBody;

        #endregion

        #region Public Methods

        public override void SetUp()
        {
            base.SetUp();

            this._template = "{validator.test}";
            this._templateBody = "validator.test";
        }

        [Test]
        public void Interpolate_AccessingTargetObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Hi [StringProperty]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { StringProperty = "Alexey" };
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.StringProperty);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Hi Alexey", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingMultipleTargetObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Hi [StringProperty] [StringProperty]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { StringProperty = "Alexey" };
            var interpolationInfo = this.CreateInterpolationInfo(testClass);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Hi Alexey Alexey", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingComplexTargetObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Hi [Complex.StringProperty]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { Complex = new { StringProperty = "Alexey" } };
            var interpolationInfo = this.CreateInterpolationInfo(testClass);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Hi Alexey", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingMultipleComplexTargetObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Hi [Complex.StringProperty]-[Complex.StringProperty]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { Complex = new { StringProperty = "Alexey" } };
            var interpolationInfo = this.CreateInterpolationInfo(testClass);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Hi Alexey-Alexey", interpolatedMessage);
        }

        [Test]
        public void IsTemplate_NullValue_False()
        {
            // arrange
            string template = null;

            // act
            var actual = this.Sut.IsTemplate(template);

            // assert
            actual.Should().Be.False();
        }

        [Test]
        public void IsTemplate_EmptyString_False()
        {
            // arrange
            string template = string.Empty;

            // act
            var actual = this.Sut.IsTemplate(template);

            // assert
            actual.Should().Be.False();
        }

        [Test]
        public void IsTemplate_MissedEnd_False()
        {
            // arrange
            string template = "{Template";

            // act
            var actual = this.Sut.IsTemplate(template);

            // assert
            actual.Should().Be.False();
        }

        [Test]
        public void IsTemplate_MissedBeggingin_False()
        {
            // arrange
            string template = "Template}";

            // act
            var actual = this.Sut.IsTemplate(template);

            // assert
            actual.Should().Be.False();
        }

        [Test]
        public void IsTemplate_Correct_False()
        {
            // arrange
            string template = "{Template}";

            // act
            var actual = this.Sut.IsTemplate(template);

            // assert
            actual.Should().Be.True();
        }

        [Test]
        public void Interpolate_AccessingValidatorObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Some $[ExtraInfo]";
            this._mock.Setup(m => m.GetFromResource(this._templateBody)).Returns(() => resource);
            var testClass = new { StringProperty = "Alexey" };
            var extraInfo = "beer";
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.StringProperty, extraInfo);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Some beer", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingMultipleValidatorObject_NoMissedReplacement()
        {
            // arrange
            var resource = "Some $[ExtraInfo]-$[ExtraInfo]!!!";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { StringProperty = "Alexey" };
            var extraInfo = "beer";
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.StringProperty, extraInfo);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Some beer-beer!!!", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingMixObjects_NoMissedReplacement()
        {
            // arrange
            var resource = "[Person] want some $[ExtraInfo]!!!";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { Person = "Alexey" };
            var extraInfo = "beer";
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.Person, extraInfo);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Alexey want some beer!!!", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingToPropertyValue_NoMissedReplacement()
        {
            // arrange
            var resource = "[PropertyValue]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { Person = "Alexey" };
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.Person);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Alexey", interpolatedMessage);
        }

        [Test]
        public void Interpolate_AccessingToPropertyName_NoMissedReplacement()
        {
            // arrange
            var resource = "[PropertyName]";
            this._mock.Setup(m => m.GetFromResource(It.IsAny<string>())).Returns(() => resource);
            var testClass = new { Person = "Alexey" };
            var interpolationInfo = this.CreateInterpolationInfo(testClass, t => t.Person);

            // act
            var interpolatedMessage = this.Sut.Interpolate(interpolationInfo);

            // assert
            Assert.AreEqual("Person", interpolatedMessage);
        }

        #endregion

        #region Methods

        protected override MessageInterpolator InitializeSut()
        {
            this._mock = new Mock<MessageInterpolator> { CallBase = true };

            return this._mock.Object;
        }

        private InterpolationInfo CreateInterpolationInfo<TTarget>(
            TTarget entityInstance, string propertyName, string extraInfo = null)
        {
            return new InterpolationInfo(
                entityInstance.GetType(), 
                entityInstance, 
                propertyName, 
                new TestValidator { ExtraInfo = extraInfo }, 
                null, 
                this._template);
        }

        private InterpolationInfo CreateInterpolationInfo<TTarget, TProperty>(
            TTarget entityInstance, Expression<Func<TTarget, TProperty>> property = null, string extraInfo = null)
        {
            return this.CreateInterpolationInfo(
                entityInstance, property == null ? null : property.Body.As<MemberExpression>().Member.Name, extraInfo);
        }

        private InterpolationInfo CreateInterpolationInfo<TTarget>(TTarget entityInstance)
        {
            return this.CreateInterpolationInfo(entityInstance, null);
        }

        #endregion

        private class TestValidator : IValidator
        {
            #region Properties

            public string ExtraInfo { get; set; }

            #endregion

            #region Implemented Interfaces

            #region IValidator

            public bool IsValid(object value, IConstraintValidatorContext constraintValidatorContext)
            {
                this.ExtraInfo = value.ToString();

                return true;
            }

            #endregion

            #endregion
        }
    }
}