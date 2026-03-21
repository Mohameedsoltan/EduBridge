namespace EduBridge.Contracts.TA;

public record TAResponse(
    Guid Id,
    string UserId,
    string FullName,
    string Email,
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxSlots,
    int AvailableSlots,
    bool IsAvailable,
    double AverageRating
);