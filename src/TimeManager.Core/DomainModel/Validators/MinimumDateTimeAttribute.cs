namespace Infocom.TimeManager.Core.DomainModel.Validators
{
    using System;

    using NHibernate.Validator.Engine;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MinimumDateTimeAttribute : Attribute, IRuleArgs, IValidator
    {
        #region Constants and Fields

        private DateTime? _minimumValueDateTime;

        #endregion

        #region Constructors and Destructors

        public MinimumDateTimeAttribute()
        {
            this.Message = "{validator.minimumDateTime}";
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        public string MinimumValue { get; set; }

        private DateTime? MinimumValueDateTime
        {
            get
            {
                if (this._minimumValueDateTime == null)
                {
                    DateTime value;
                    if (DateTime.TryParse(this.MinimumValue, out value))
                    {
                        this._minimumValueDateTime = value;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                "MinimumValue with value '{0}' can not be parsed to DateTime.", this.MinimumValue));
                    }
                }

                return this._minimumValueDateTime;
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
                return this.MinimumValueDateTime < objectToValidate;
            }

            return false;
        }

        #endregion

        #endregion
    }
}