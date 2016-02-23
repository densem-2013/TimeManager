namespace Infocom.TimeManager.Core.DomainModel.Validators
{
    using System;

    using NHibernate.Validator.Engine;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MaximumDateTimeAttribute : Attribute, IRuleArgs, IValidator
    {
        #region Constants and Fields

        private DateTime? _maximumValueDateTime;

        #endregion

        #region Constructors and Destructors

        public MaximumDateTimeAttribute()
        {
            this.Message = "{validator.maximumDateTime}";
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        public string MaximumValue { get; set; }

        private DateTime? MaximumValueDateTime
        {
            get
            {
                if (this._maximumValueDateTime == null)
                {
                    DateTime value;
                    if (DateTime.TryParse(this.MaximumValue, out value))
                    {
                        this._maximumValueDateTime = value;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "MaximumValue with value '{0}' can not be parsed to DateTime.", this.MaximumValue));
                    }
                }

                return this._maximumValueDateTime;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IValidator

        public bool IsValid(object value, IConstraintValidatorContext constraintValidatorContext)
        {
            if (value == null)
            {
                return false;
            }

            DateTime objectToValidate;
            if (DateTime.TryParse(value.ToString(), out objectToValidate))
            {
                return this.MaximumValueDateTime > objectToValidate;
            }

            return false;
        }

        #endregion

        #endregion
    }
}