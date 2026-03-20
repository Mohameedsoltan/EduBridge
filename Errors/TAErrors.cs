using EduBridge.Abstractions;

namespace EduBridge.Errors;

public static class TaErrors
{
    public static readonly Error TaNotFound = new(
        "TA.NotFound", "Teaching assistant not found", StatusCodes.Status404NotFound);

    public static readonly Error TaNotAvailable = new(
        "TA.NotAvailable", "This teaching assistant has no available slots", StatusCodes.Status409Conflict);

    public static readonly Error NotAuthorized = new(
        "TA.NotAuthorized", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
}