namespace EduBridge.Contracts.TA;

public record TAResponse(
    string UserId,
    string FirstName,
    string LastName,
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxSlots,
    int AvailableSlots,
    bool IsAvailable,
    double AverageRating
);