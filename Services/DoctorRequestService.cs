using EduBridge.Abstractions;
using EduBridge.Abstractions.Consts;
using EduBridge.Contracts.Doctor;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class DoctorRequestService(
    ApplicationDbContext context,
    INotificationService notificationService,
    IMapper mapper) : IDoctorRequestService
{
    public async Task<Result> CreateRequestAsync(
        SendDoctorRequestRequest request, CancellationToken cancellationToken = default)
    {
        var doctor = await context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (doctor is null)
            return Result.Failure(DoctorErrors.DoctorNotFound);

        if (!doctor.IsAvailable)
            return Result.Failure(DoctorErrors.DoctorNotAvailable);

        var existingRequest = await context.DoctorRequests
            .AnyAsync(r => r.TeamId == request.TeamId
                && r.DoctorId == request.DoctorId
                && r.Status == RequestStatus.Pending, cancellationToken);

        if (existingRequest)
            return Result.Failure(DoctorRequestErrors.RequestAlreadyExists);

        var doctorRequest = new DoctorRequest
        {
            TeamId = request.TeamId,
            DoctorId = request.DoctorId,
            Message = request.Message,
            Status = RequestStatus.Pending
        };

        await context.DoctorRequests.AddAsync(doctorRequest, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.SendAsync(
            doctor.UserId,
            NotificationType.DoctorRequestReceived,
            "You have received a new doctor request from a team.",
            doctorRequest.Id,
            cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RespondToRequestAsync(
        Guid requestId, string doctorUserId, RespondDoctorRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var doctorRequest = await context.DoctorRequests
            .Include(r => r.Doctor)
            .Include(r => r.Team)
            .FirstOrDefaultAsync(r => r.Id == requestId, cancellationToken);

        if (doctorRequest is null)
            return Result.Failure(DoctorRequestErrors.RequestNotFound);

        if (doctorRequest.Doctor.UserId != doctorUserId)
            return Result.Failure(DoctorRequestErrors.Unauthorized);

        if (doctorRequest.Status != RequestStatus.Pending)
            return Result.Failure(DoctorRequestErrors.RequestNotPending);

        doctorRequest.Status = request.IsApproved ? RequestStatus.Approved : RequestStatus.Rejected;
        doctorRequest.ResponseMessage = request.ResponseMessage;
        doctorRequest.RespondedAt = DateTime.UtcNow;
        doctorRequest.RespondedByDoctorId = doctorRequest.DoctorId;

        if (request.IsApproved)
        {
            doctorRequest.Doctor.AvailableTeams--;
            doctorRequest.Team.DoctorId = doctorRequest.DoctorId;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<DoctorRequestResponse>>> GetTeamRequestsAsync(
        Guid teamId, CancellationToken cancellationToken = default)
    {
        var requests = await context.DoctorRequests
            .AsNoTracking()
            .Include(r => r.Team)
            .Include(r => r.Doctor).ThenInclude(d => d.User)
            .Where(r => r.TeamId == teamId)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DoctorRequestResponse>>(mapper.Map<IEnumerable<DoctorRequestResponse>>(requests));
    }

    public async Task<Result<IEnumerable<DoctorRequestResponse>>> GetDoctorRequestsAsync(
        Guid doctorId, CancellationToken cancellationToken = default)
    {
        var requests = await context.DoctorRequests
            .AsNoTracking()
            .Include(r => r.Team)
            .Include(r => r.Doctor).ThenInclude(d => d.User)
            .Where(r => r.DoctorId == doctorId)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DoctorRequestResponse>>(mapper.Map<IEnumerable<DoctorRequestResponse>>(requests));
    }
}