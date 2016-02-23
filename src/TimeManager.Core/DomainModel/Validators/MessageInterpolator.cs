namespace Infocom.TimeManager.Core.DomainModel.Validators
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    using Infocom.TimeManager.Core.DomainModel.Repositories;
    using Infocom.TimeManager.Core.Resources;

    using NHibernate.Validator.Engine;

    public class MessageInterpolator : IMessageInterpolator
    {
        #region Constants and Fields

        private readonly ResourceManager resourceManager;

        #endregion

        #region Constructors and Destructors

        public MessageInterpolator()
        {
            this.resourceManager = ValidationMessages.ResourceManager;
        }

        #endregion

        #region Public Methods

        public virtual string GetFromResource(string resourceName)
        {
            return this.resourceManager.GetString(resourceName, CultureInfo.CurrentCulture);
        }

        public virtual bool IsTemplate(string message)
        {
            return !string.IsNullOrWhiteSpace(message) && message.StartsWith("{") && message.EndsWith("}");
        }

        #endregion

        #region Implemented Interfaces

        #region IMessageInterpolator

        public string Interpolate(InterpolationInfo info)
        {
            if (!this.IsTemplate(info.Message))
            {
                return info.Message;
            }

            var templateBody = this.ExtractTemplateBody(info.Message);

            var resolvedMessage = this.GetFromResource(templateBody);

            if (string.IsNullOrEmpty(resolvedMessage))
            {
                // Returns the original message
                return info.Message;
            }

            if (info.Validator != null)
            {
                resolvedMessage = this.Replace(info.Validator.GetType(), info.Validator, resolvedMessage, "$[", "]");
            }

            if (resolvedMessage.Contains("[PropertyValue]"))
            {
                var resolvedProperty = this.GetPropertyValue(info.Entity, info.EntityInstance, info.PropertyName);
                string propertyValue = String.Empty;
                if (resolvedProperty != null)
                {
                    propertyValue = resolvedProperty.ToString();
                }

                resolvedMessage = this.Replace(resolvedMessage, "[PropertyValue]", propertyValue);
            }

            if (resolvedMessage.Contains("[PropertyName]"))
            {
                resolvedMessage = this.Replace(resolvedMessage, "[PropertyName]", info.PropertyName);
            }

            resolvedMessage = this.Replace(info.Entity, info.EntityInstance, resolvedMessage, "[", "]");

            return resolvedMessage;
        }

        #endregion

        #endregion

        #region Methods

        private PropertyInfo GetProperty(Type targetType, string propertyName)
        {
            PropertyInfo result = targetType.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance);
            if (result == null)
            {
                throw new ObjectNotExistException(String.Format("Property '{0}' is not found.", propertyName));
            }

            return result;
        }

        private object GetPropertyValue(Type targetType, object target, string propertyName)
        {
            return this.GetProperty(targetType, propertyName).GetValue(target, null);
        }

        private string ExtractTemplateBody(string message)
        {
            return message.Substring(1, message.Length - 2);
        }

        private string Replace(Type entity, object entityInstance, string message, string prefix, string postfix)
        {
            while (message.Contains(prefix))
            {
                var startPos = message.IndexOf(prefix) + prefix.Length;
                var endPos = message.IndexOf(postfix, startPos);
                var propertyPath = message.Substring(startPos, endPos - startPos);
                var propertyPathItems = propertyPath.Split('.');

                object valueToInsert = this.GetPropertyValue(entity, entityInstance, propertyPathItems.First());

                for (int i = 1; i < propertyPathItems.Length; i++)
                {
                    valueToInsert = this.GetPropertyValue(valueToInsert.GetType(), valueToInsert, propertyPathItems[i]);
                }

                message = message.Remove(startPos - prefix.Length, endPos - startPos + prefix.Length + postfix.Length);
                message = message.Insert(startPos - prefix.Length, valueToInsert.ToString());
            }

            return message;
        }

        private string Replace(string message, string fromString, string toString)
        {
            while (message.Contains(fromString))
            {
                var startPos = message.IndexOf(fromString);
                var length = fromString.Length;

                message = message.Remove(startPos, length);
                message = message.Insert(startPos, toString);
            }

            return message;
        }

        #endregion
    }
}