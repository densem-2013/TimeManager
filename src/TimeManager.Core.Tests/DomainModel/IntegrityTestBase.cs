namespace Infocom.TimeManager.Core.Tests.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Infocom.TimeManager.Core.DomainModel.Validators;

    using NHibernate.Linq;
    using NHibernate.Validator.Constraints;
    using NHibernate.Validator.Engine;
    using NHibernate.Validator.Exceptions;

    using Ninject;
    using Ninject.MockingKernel;

    using NUnit.Framework;

    [TestFixture]
    public abstract class IntegrityTestBase<T> : BaseTest<T>
        where T : class
    {
        #region Constructors and Destructors

        public IntegrityTestBase()
        {
            var validator = new ValidatorEngine();
            validator.Configure();

            this.MockingKernel.Rebind<ValidatorEngine>().ToConstant(validator);
            this.MockingKernel.Rebind<Validation.IFor>().To<Validation.AttributeCheckingFor>();
        }

        #endregion

        #region Methods

        protected Validation CheckProperty<TProperty>(
            Expression<Func<T, TProperty>> propertyToValidate, params object[] activeTags)
        {
            if (activeTags != null && activeTags.Length == 0)
            {
                activeTags = null;
            }

            var objectToValidate = propertyToValidate.Compile().Invoke(this.Sut);

            return new Validation(
                this.MockingKernel.Get<ValidatorEngine>(),
                this.Sut,
                objectToValidate,
                propertyToValidate.Body.As<MemberExpression>().Member.As<PropertyInfo>(),
                activeTags,
                this.MockingKernel);
        }

        #endregion

        public class Validation
        {
            #region Constants and Fields

            private readonly object _activeTags;

            private readonly MockingKernel _container;

            private readonly ValidatorEngine _engine;

            private readonly bool _isTargetPropertyValueType;

            private readonly object _target;

            private readonly PropertyInfo _targetProperty;

            private readonly object _targetPropertyValue;

            #endregion

            #region Constructors and Destructors

            public Validation(
                ValidatorEngine engine,
                object target,
                object targetPropertyValue,
                PropertyInfo targetProperty,
                object[] activeTags,
                MockingKernel container)
            {
                this._engine = engine;
                this._target = target;
                this._activeTags = activeTags;
                this._container = container;
                this._targetPropertyValue = targetPropertyValue;
                this._targetProperty = targetProperty;
                this._isTargetPropertyValueType = this._targetPropertyValue is ValueType;
            }

            #endregion

            #region Interfaces

            public interface IFor
            {
                #region Properties

                Validation Validation { get; set; }

                #endregion

                #region Public Methods

                void NotNull();

                void Null();

                void MinimumDateTime(DateTime dateTime);

                void MaximumDateTime(DateTime dateTime);

                #endregion
            }

            #endregion

            #region Properties

            public IFor For
            {
                get
                {
                    var result = this._container.Get<IFor>();
                    result.Validation = this;

                    return result;
                }
            }

            #endregion

            public class AttributeCheckingFor : IFor
            {
                #region Properties

                public Validation Validation { get; set; }

                #endregion

                #region Implemented Interfaces

                #region IFor

                public void MaximumDateTime(DateTime dateTime)
                {
                    var attribute = this.Validation._targetProperty.GetAttribute<MaximumDateTimeAttribute>();
                    var currentValue = DateTime.Parse(attribute.MaximumValue);

                    if (!(currentValue == dateTime))
                    {
                        throw new ApplicationException(
                            string.Format("Expected value: '{0}'. Current value: '{1}'", dateTime, currentValue));
                    }
                }

                public void MinimumDateTime(DateTime dateTime)
                {
                    var attribute = this.Validation._targetProperty.GetAttribute<MinimumDateTimeAttribute>();
                    var currentValue = DateTime.Parse(attribute.MinimumValue);

                    if (!(currentValue == dateTime))
                    {
                        throw new ApplicationException(
                            string.Format("Expected value: '{0}'. Current value: '{1}'", dateTime, currentValue));
                    }
                }

                public void NotNull()
                {
                    this.Validation._targetProperty.GetAttribute<NotNullAttribute>();
                }

                public void Null()
                {
                    try
                    {
                        this.Validation._targetProperty.GetAttribute<NotNullAttribute>();
                        throw new ApplicationException("Attribute 'NotNullAttribute' should be not fould");
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("'NotNullAttribute' attribute is not found."))
                        {
                            throw;
                        }
                    }
                }

                #endregion

                #endregion
            }

            public class DirectForImp : IFor
            {
                #region Properties

                public Validation Validation { get; set; }

                #endregion

                #region Implemented Interfaces

                #region IFor

                public void MaximumDateTime(DateTime dateTime)
                {
                    var invalidDateTime = dateTime.AddTicks(1);
                    this.Validate(
                        p => p.SetValue(this.Validation._target, invalidDateTime, null),
                        iv => iv.Count() > 0,
                        iv => "Property '{0}' does not have 'MaximumDateTime' validation for {1}.",
                        p => p.Name,
                        p => dateTime);
                }

                public void MinimumDateTime(DateTime dateTime)
                {
                    var invalidDateTime = dateTime.AddTicks(-1);
                    this.Validate(
                        p => p.SetValue(this.Validation._target, invalidDateTime, null),
                        iv => iv.Count() > 0,
                        iv => "Property '{0}' does not have 'MinimumDateTime' validation for {1}. ",
                        p => p.Name,
                        p => dateTime);
                }

                public void NotNull()
                {
                    this.Validate(
                        p => p.SetValue(this.Validation._target, null, null),
                        iv => iv.Count() > 0,
                        iv => "Property '{0}' does not have 'NotNull' validation.",
                        p => p.Name);
                }

                public void Null()
                {
                    this.Validate(
                        p => p.SetValue(this.Validation._target, null, null),
                        iv => iv.Count() == 0,
                        iv => "Property '{0}' does have 'NotNull' validation.",
                        p => p.Name);
                }

                #endregion

                #endregion

                #region Methods

                private void Validate(
                    Action<PropertyInfo> act,
                    Func<IEnumerable<InvalidValue>, bool> assert,
                    Func<IEnumerable<InvalidValue>, string> failMessage,
                    params Func<PropertyInfo, object>[] failMessageArgs)
                {
                    Func<PropertyInfo, bool> propertyFilterPredicate = p =>
                        {
                            var targetMember = p.GetValue(this.Validation._target, null);
                            if (!this.Validation._isTargetPropertyValueType)
                            {
                                return this.Validation._targetPropertyValue == targetMember;
                            }

                            return this.Validation._targetPropertyValue.Equals(targetMember);
                        };
                    var foundProperties = typeof(T).GetProperties().Where(propertyFilterPredicate).ToArray();

                    if (foundProperties.Length == 1)
                    {
                        var property = foundProperties.Single();

                        try
                        {
                            act(property);
                        }
                        catch (ArgumentException ex)
                        {
                            throw new ApplicationException("Act stage failed, probably type mismatch", ex);
                        }

                        try
                        {
                            var invalidValues = this.Validation._engine.ValidatePropertyValue(
                                this.Validation._target, property.Name, this.Validation._activeTags);
                            if (!assert(invalidValues))
                            {
                                Assert.Fail(
                                    failMessage(invalidValues), failMessageArgs.Select(fma => fma(property)).ToArray());
                            }
                        }
                        catch (HibernateValidatorException e)
                        {
                        }
                    }
                    else if (foundProperties.Length == 0)
                    {
                        throw new Exception("Property not found, check assertion expression.");
                    }
                    else
                    {
                        throw new Exception("Property cannot be identified because of multiple choices.");
                    }
                }

                #endregion
            }
        }
    }

    public static class Extensions
    {
        #region Public Methods

        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo target)
        {
            var result = target.GetCustomAttributes(typeof(TAttribute), true).As<TAttribute[]>().FirstOrDefault();
            if (result == null)
            {
                throw new ApplicationException(String.Format("'{0}' attribute is not found.", typeof(TAttribute).Name));
            }

            return result;
        }

        #endregion
    }
}