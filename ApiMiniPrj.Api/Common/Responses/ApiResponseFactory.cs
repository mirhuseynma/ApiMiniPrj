namespace ApiMiniPrj.Api.Common.Responses
{
    public static class ApiResponseFactory
    {
        public static BadRequestObjectResult ValidationError(FluentValidation.Results.ValidationResult validationResult, HttpContext? httpContext)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(e => e.ErrorMessage).ToArray());

            return new BadRequestObjectResult(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Errors = errors,
                TraceId = httpContext?.TraceIdentifier
            });
        }

        public static BadRequestObjectResult BadRequest(string message, HttpContext? httpContext)
        {
            return new BadRequestObjectResult(new ApiErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = message,
                TraceId = httpContext?.TraceIdentifier
            });
        }
    }
}
