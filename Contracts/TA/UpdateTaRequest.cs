namespace EduBridge.Contracts.TA;

public record UpdateTaRequest(
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxSlots,
    int AvailableSlots
);