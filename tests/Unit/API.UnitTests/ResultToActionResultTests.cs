using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace API.UnitTests
{
    public class ResultToActionResultTests
    {
        private class DummyController : ControllerBase { }

        [Fact]
        public void SuccessResult_Returns_Ok_With_ApiSuccessResponse()
        {
            var controller = new DummyController();
            var result = Result.Success<int>(42);

            var action = result.ToActionResult(controller) as OkObjectResult;
            Assert.NotNull(action);
            var body = Assert.IsType<Hello100Admin.BuildingBlocks.Common.Errors.ApiSuccessResponse<int>>(action.Value);
            Assert.Equal(42, body.Data);
        }

        [Fact]
        public void UserNotFound_Returns_NotFound()
        {
            var controller = new DummyController();
            var result = Result.Failure<string>("no user", ErrorCodes.UserNotFound);

            var action = result.ToActionResult(controller);
            Assert.IsType<NotFoundObjectResult>(action);
        }

        [Fact]
        public void ValidationError_Returns_BadRequest()
        {
            var controller = new DummyController();
            var result = Result.Failure<string>("invalid", ErrorCodes.Validation);

            var action = result.ToActionResult(controller);
            Assert.IsType<BadRequestObjectResult>(action);
        }

        [Fact]
        public void AuthFailed_WithAuthEndpoint_Returns_Unauthorized()
        {
            var controller = new DummyController();
            var result = Result.Failure<string>("auth failed", ErrorCodes.AuthFailed);

            var action = result.ToActionResult(controller, authEndpoint: true);
            Assert.IsType<UnauthorizedObjectResult>(action);
        }
    }
}
