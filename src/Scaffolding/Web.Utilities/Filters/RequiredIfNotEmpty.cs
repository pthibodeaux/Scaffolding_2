using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Scaffolding.Web.Utilities.Filters
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public sealed class RequiredIfNotEmptyAttribute : RequiredAttribute
	{
		public string[] OtherPropertyNames { get; set; }

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			object instance = context.ObjectInstance;
			Type type = instance.GetType();

			List<string> otherPropertyValues = OtherPropertyNames
				.Select(otherPropertyName => type.GetProperty(otherPropertyName).GetValue(instance, null))
				.Select(otherPropertyValue => otherPropertyValue?.ToString()).ToList();

			foreach (var otherPropertyValue in otherPropertyValues)
			{
				if (!string.IsNullOrWhiteSpace(otherPropertyValue))
				{
					ValidationResult result = base.IsValid(value, context);
					return result;
				}
			}

			return ValidationResult.Success;
		}
	}
}
