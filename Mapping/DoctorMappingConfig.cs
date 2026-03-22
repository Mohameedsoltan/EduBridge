using EduBridge.Contracts.Doctor;
using EduBridge.Entities;
using Mapster;

namespace EduBridge.Mapping;

public class DoctorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Doctor, DoctorResponse>()
            .Map(dest => dest.FullName, src => $"{src.User.FirstName} {src.User.LastName}")
            .Map(dest => dest.Email, src => src.User.Email);

        config.NewConfig<CreateDoctorRequest, Doctor>()
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.AvailableTeams);

        config.NewConfig<UpdateDoctorRequest, Doctor>()
            .Ignore(dest => dest.AvailableTeams)
            .Ignore(dest => dest.UserId);
    }
}