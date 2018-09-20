﻿#if NET462
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.Mvc.UnitTests
{
    [TestFixture]
    public class ValidateModelStateFilterTests : FluentTest<ValidateModelStateFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsValid_ThenShouldNotSetResult()
        {
            Run(f => f.SetPostRequest(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetTempDataModelState()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.Controller.Object.TempData[typeof(SerializableModelStateDictionary).FullName].Should().NotBeNull());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetRedirectToRouteResult()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().NotBeNull().And.Match<RedirectToRouteResult>(r => r.RouteValues == f.RouteData.Values));
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsValid_ThenModelStateShouldBeValid()
        {
            Run(f => f.SetGetRequest(), f => f.OnActionExecuting(), f => f.Controller.Object.ViewData.ModelState.IsValid.Should().BeTrue());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsInvalid_ThenModelStateShouldBeInvalid()
        {
            Run(f => f.SetGetRequest().SetInvalidTempDataModelState(), f => f.OnActionExecuting(), f => f.Controller.Object.ViewData.ModelState.IsValid.Should().BeFalse());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndNoValidationExceptionHasBeenThrown_ThenShouldNotSetResult()
        {
            Run(f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<EmptyResult>());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndAValidationExceptionHasBeenThrown_ThenShouldSetTempDataModelState()
        {
            Run(f => f.SetValidationException(), f => f.OnActionExecuted(), f => f.Controller.Object.TempData[typeof(SerializableModelStateDictionary).FullName].Should().NotBeNull());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndAValidationExceptionHasBeenThrown_ThenShouldSetResult()
        {
            Run(f => f.SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.Match<RedirectToRouteResult>(r => r.RouteValues == f.RouteData.Values));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndAValidationExceptionHasBeenThrown_ThenShouldSetExceptionHandled()
        {
            Run(f => f.SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.ExceptionHandled.Should().BeTrue());
        }
    }

    public class ValidateModelStateFilterTestsFixture
    {
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public ValidateModelStateFilter ValidateModelStateFilter { get; set; }
        public Mock<HttpContextBase> HttpContext { get; set; }
        public Mock<ControllerBase> Controller { get; set; }
        public RouteData RouteData { get; set; }


        public ValidateModelStateFilterTestsFixture()
        {
            HttpContext = new Mock<HttpContextBase>();
            Controller = new Mock<ControllerBase>();
            RouteData = new RouteData();

            ActionExecutingContext = new ActionExecutingContext
            {
                HttpContext = HttpContext.Object,
                Controller = Controller.Object,
                RouteData = RouteData
            };

            ActionExecutedContext = new ActionExecutedContext
            {
                HttpContext = HttpContext.Object,
                Controller = Controller.Object,
                RouteData = RouteData
            };

            ValidateModelStateFilter = new ValidateModelStateFilter();

            Controller.Object.ViewData.ModelState.SetModelValue("Foo", new ValueProviderResult("FooRawValue", "FooAttemptedValue", CultureInfo.InvariantCulture));
            Controller.Object.ViewData.ModelState.SetModelValue("Bar", new ValueProviderResult("BarRawValue", "BarAttemptedValue", CultureInfo.InvariantCulture));
            HttpContext.Setup(c => c.Request.QueryString).Returns(new NameValueCollection());
        }

        public void OnActionExecuting()
        {
            ValidateModelStateFilter.OnActionExecuting(ActionExecutingContext);
        }

        public void OnActionExecuted()
        {
            ValidateModelStateFilter.OnActionExecuted(ActionExecutedContext);
        }

        public ValidateModelStateFilterTestsFixture SetGetRequest()
        {
            HttpContext.Setup(c => c.Request.HttpMethod).Returns("GET");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetValidationException()
        {
            ActionExecutedContext.Exception = new ValidationException();

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetPostRequest()
        {
            HttpContext.Setup(c => c.Request.HttpMethod).Returns("POST");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetInvalidModelState()
        {
            Controller.Object.ViewData.ModelState.AddModelError("Foo", "FooErrorMessage");
            Controller.Object.ViewData.ModelState.AddModelError("Bar", "BarErrorMessage");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetInvalidTempDataModelState()
        {
            Controller.Object.TempData[typeof(SerializableModelStateDictionary).FullName] = new SerializableModelStateDictionary
            {
                Data = new List<SerializableModelState>
                {
                    new SerializableModelState
                    {
                        AttemptedValue = "FooAttemptedValue",
                        ErrorMessages = new List<string> { "FooErrorMessage" },
                        Key = "Foo",
                        RawValue = "FooRawValue"
                    },
                    new SerializableModelState
                    {
                        AttemptedValue = "BarAttemptedValue",
                        ErrorMessages = new List<string> { "BarErrorMessage" },
                        Key = "Bar",
                        RawValue = "BarRawValue"
                    }
                }
            };

            return this;
        }
    }
}
#endif