namespace EduBridge.Contracts.TA;
public record CreateTaRequest(
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxSlots
);
