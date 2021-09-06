using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Data.IO.Attributes
{
    /// <summary>
    /// Validation attribute to assert an <see cref="IFormFile">IFormFile</see> property, field or parameter has a specific extention.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FileExtensionsAttribute : ValidationAttribute
    {
        private string _extensions;

        /// <summary>
        /// Gets or sets the acceptable extensions of the file.
        /// </summary>
        public string Extensions
        {
            get
            {
                // Default file extensions match those from jquery validate.
                return string.IsNullOrEmpty(_extensions) ? "png,jpg,jpeg,gif" : _extensions;
            }
            set
            {
                _extensions = value;
            }
        }

        private string ExtensionsNormalized
        {
            get
            {
                return Extensions.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant();
            }
        }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public FileExtensionsAttribute() : base(() => "")
        { }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.IsValid(object)"/>
        /// </summary>
        /// <remarks>
        /// This method returns <c>true</c> if the <paramref name="value"/> is null.
        /// It is assumed the <see cref="RequiredAttribute"/> is used if the value may not be null.
        /// </remarks>
        /// <param name="value">The value to test.</param>
        /// <returns><c>true</c> if the value is null or it's extension is not invluded in the set extensionss</returns>
        public override bool IsValid(object value)
        {
            // Automatically pass if value is null. RequiredAttribute should be used to assert a value is not null.
            if (value == null)
            {
                return true;
            }

            // We expect a cast exception if the passed value was not an IFormFile.
            return ExtensionsNormalized.Split(",").Contains(Path.GetExtension(((IFormFile)value).FileName).ToUpperInvariant());
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.FormatErrorMessage"/>
        /// </summary>
        /// <param name="name">The name to include in the formatted string</param>
        /// <returns>A localized string to describe the acceptable extensions</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Extensions);
        }
    }
}