using EduBridge.Contracts.Doctor;
using Mapster;

namespace EduBridge.Mapping;

public class DoctorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Doctor, DoctorResponse>()
            .Map(dest => dest.FullName, src => $"{src.User.FirstName} {src.User.LastName}");
    }
}