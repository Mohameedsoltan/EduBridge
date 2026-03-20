using EduBridge.Abstractions;
using EduBridge.Contracts.Doctor;
using EduBridge.Entities;
using EduBridge.Errors;
using EduBridge.Persistence;
using EduBridge.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduBridge.Services;

public class DoctorService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IMapper mapper) : IDoctorService
{
    public async Task<Result<IEnumerable<DoctorResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var doctors = await context.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DoctorResponse>>(mapper.Map<IEnumerable<DoctorResponse>>(doctors));
    }

    public async Task<Result<DoctorResponse>> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await context.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (doctor is null)
            return Result.Failure<DoctorResponse>(DoctorErrors.DoctorNotFound);

        return Result.Success(mapper.Map<DoctorResponse>(doctor));
    }

    public async Task<Result<IEnumerable<DoctorResponse>>> GetAvailableDoctorsAsync(
        CancellationToken cancellationToken = default)
    {
        var doctors = await context.Doctors
            .AsNoTracking()
            .Include(d => d.User)
            .Where(d => d.AvailableTeams > 0)
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DoctorResponse>>(mapper.Map<IEnumerable<DoctorResponse>>(doctors));
    }

    public async Task<Result> CreateAsync(
        CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return Result.Failure(DoctorErrors.UserNotFound);

        var alreadyDoctor = await context.Doctors
            .AnyAsync(d => d.UserId == request.UserId, cancellationToken);

        if (alreadyDoctor)
            return Result.Failure(DoctorErrors.UserAlreadyDoctor);

        var doctor = new Doctor
        {
            UserId = request.UserId,
            Department = request.Department,
            AcademicTitle = request.AcademicTitle,
            OfficeLocation = request.OfficeLocation,
            MaxTeams = request.MaxTeams,
            AvailableTeams = request.MaxTeams
        };

        await context.Doctors.AddAsync(doctor, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(
        Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var doctor = await context.FindAsync<Doctor>([id], cancellationToken);

        if (doctor is null || doctor.IsDeleted)
            return Result.Failure(DoctorErrors.DoctorNotFound);

        doctor.Department = request.Department;
        doctor.AcademicTitle = request.AcademicTitle;
        doctor.OfficeLocation = request.OfficeLocation;
        doctor.MaxTeams = request.MaxTeams;
        doctor.AvailableTeams = request.AvailableTeams;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var doctor = await context.FindAsync<Doctor>([id], cancellationToken);

        if (doctor is null || doctor.IsDeleted)
            return Result.Failure(DoctorErrors.DoctorNotFound);

        doctor.IsDeleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}