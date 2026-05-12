namespace UsersProducts.Api.Common.Errors;

public sealed record ApiErrorResponse(
    int StatusCode,
    string Error,
    string Message,
    string Path,
    DateTime TimestampUtc
);