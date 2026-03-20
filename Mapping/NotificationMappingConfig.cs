using EduBridge.Contracts.Notification;
using EduBridge.Entities;
using Mapster;

namespace EduBridge.Mapping;

public class NotificationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Notification, NotificationResponse>()
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
    }
}