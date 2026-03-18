namespace EduBridge.Contracts.Doctor;
public record CreateDoctorRequest(
    string UserId,
    string Department,
    string? AcademicTitle,
    string? OfficeLocation,
    int MaxTeams
);
