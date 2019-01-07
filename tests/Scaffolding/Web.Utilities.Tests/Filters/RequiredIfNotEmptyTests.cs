using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Scaffolding.Web.Utilities.Filters;
using Xunit;

namespace Scaffolding.Web.Utilities.Tests.Filters
{
    public class RequiredIfNotEmptyTests
    {
	    private class RequiredIfNotEmptyAttributeTestModel
	    {
		    [RequiredIfNotEmpty(OtherPropertyNames = new []{nameof(DependencyProp)})]
		    public string TestProp { get; set; }
		    public string DependencyProp { get; set; }
	    }

	    [Fact]
	    public void RequiredIf_Property_Should_Be_Valid_If_Condition_Met()
	    {
		    var model = new RequiredIfNotEmptyAttributeTestModel
		    {
			    DependencyProp = "Some Other String",
			    TestProp = "some string"
		    };

		    var isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
		    isValid.Should().BeTrue();
	    }

	    [Fact]
	    public void RequiredIf_Property_Should_Be_Invalid_If_Condition_Not_Met()
	    {
		    var model = new RequiredIfNotEmptyAttributeTestModel
		    {
			    DependencyProp = "Some Other String",
			    TestProp = null
		    };

		    var isValid = Validator.TryValidateObject(model, new ValidationContext(model), new List<ValidationResult>(), true);
		    isValid.Should().BeFalse();
	    }

	    [Fact]
	    public void RequiredIf_Property_Should_Be_Valid_If_Not_Required()
	    {
		    var model = new RequiredIfNotEmptyAttributeTestModel
		    {
			    DependencyProp = string.Empty,
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
