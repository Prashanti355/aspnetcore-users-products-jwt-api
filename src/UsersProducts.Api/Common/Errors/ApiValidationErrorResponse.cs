namespace UsersProducts.Api.Common.Errors;

public sealed record ApiValidationErrorResponse(
    int StatusCode,
    string Error,
    string Message,
    string Path,
    DateTime TimestampUtc,
    IDictionary<string, string[]> ValidationErrors
);