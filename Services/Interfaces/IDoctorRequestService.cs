using EduBridge.Abstractions;
using EduBridge.Contracts.Doctor;

namespace EduBridge.Services.Interfaces;

public interface IDoctorRequestService
{
    // Create request
    Task<Result> CreateRequestAsync(SendDoctorRequestRequest request, CancellationToken cancellationToken = default);

    // Respond (Approve / Reject)
    Task<Result> RespondToRequestAsync(Guid requestId, string doctorUserId, RespondDoctorRequestDto request, CancellationToken cancellationToken = default);

    // Queries
    Task<Result<IEnumerable<DoctorRequestResponse>>> GetTeamRequestsAsync(Guid teamId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DoctorRequestResponse>>> GetDoctorRequestsAsync(Guid doctorId, CancellationToken cancellationToken = default);
}