using EduBridge.Contracts.TA;
using EduBridge.Entities;
using Mapster;

namespace EduBridge.Persistence.EntitiesConfiguration;

public class TaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TeachingAssistant, TAResponse>()
            .Map(dest => dest.FullName, src => $"{src.User.FirstName} {src.User.LastName}")
            .Map(dest => dest.Email, src => src.User.Email)
            .Map(dest => dest.UserId, src => src.UserId);

        config.NewConfig<CreateTaRequest, TeachingAssistant>()
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.AvailableSlots);

        config.NewConfig<UpdateTaRequest, TeachingAssistant>()
            .Ignore(dest => dest.AvailableSlots)
            .Ignore(dest => dest.UserId);
    }
}