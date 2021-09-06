using Services.Common.MethodResultUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Services.Common.Domain.Entities
{
    public abstract class EntityValidator
    {
        protected Assembly GetAssembly() => GetType().Assembly;

        protected List<ErrorResult> _errorMessages = new();

        protected List<WarningResult> _warningResults = new();

        /// <summary>
        /// List of ErrorResult.
        /// </summary>
        [NotMapped] protected IReadOnlyCollection<ErrorResult> ErrorMessages => _errorMessages;

        /// <summary>
        /// List of WarningResult.
        /// </summary>
        [NotMapped] protected IReadOnlyCollection<WarningResult> WarningResults => _warningResults;

        /// <summary>
        /// Check If entity is valid or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            var context = new ValidationContext(this, null, null);

            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, context, results, true);

            if (!isValid)
            {
                foreach (var result in results)
                {
                    var errorResult = new ErrorResult
                    {
                        ErrorCode = result.ErrorMessage
                    };

                    errorResult.ErrorMessage = errorResult.ErrorCode.StartsWith(Settings.CommonErrorPrefix, StringComparison.InvariantCulture) ? ErrorHelpers.GetCommonErrorMessage(result.ErrorMessage) : ErrorHelpers.GetErrorMessage(result.ErrorMessage, GetAssembly());

                    foreach (var property in result.MemberNames)
                    {
                        var propertyInfo = context.ObjectType.GetProperty(property);

                        var value = propertyInfo.GetValue(context.ObjectInstance, null);

                        errorResult.ErrorValues.Add(ErrorHelpers.GenerateErrorResult(property, value));
                    }
                    _errorMessages.Add(errorResult);
                }
            }

            return _errorMessages.Count == 0;
        }
    }
}