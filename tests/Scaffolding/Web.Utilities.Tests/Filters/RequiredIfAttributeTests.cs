using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Scaffolding.Web.Utilities.Filters;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Filters
{
	public class RequiredIfAttributeTests
	{
		private class RequiredIfAttributeTestModel
		{
			[RequiredIf("DependencyProp", true)]
			public string TestProp { get; set; }
			public bool DependencyProp { get; set; }
		}

		[Fact]
		public void RequiredIf_Property_Should_Be_Valid_If_Condition_Met()
		{
			var model = new RequiredIfAttributeTestModel
			{
				DependencyProp = true,
				TestProp = "some string"
			};

			var isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
			isValid.Should().BeTrue();
		}

		[Fact]
		public void RequiredIf_Property_Should_Be_Invalid_If_Condition_Not_Met()
		{
			var model = new RequiredIfAttributeTestModel
			{
				DependencyProp = true,
				TestProp = null
			};

			var isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
			isValid.Should().BeFalse();
		}

		[Fact]
		public void RequiredIf_Property_Should_Be_Valid_If_Not_Required()
		{
			var model = new RequiredIfAttributeTestModel
			{
				DependencyProp = false,
				TestProp = null
			};
			var isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
			isValid.Should().BeTrue();

			model.TestProp = "some string";
			isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
			isValid.Should().BeTrue();
		}
	}
}
