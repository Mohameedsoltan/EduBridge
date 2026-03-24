namespace EduBridge.Contracts.Doctor;

public record CreateDoctorRequest(
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxTeams
);