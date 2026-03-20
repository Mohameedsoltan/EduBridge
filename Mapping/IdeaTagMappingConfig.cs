using EduBridge.Contracts.Idea;
using EduBridge.Entities;
using Mapster;

namespace EduBridge.Mapping;

public class IdeaTagMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IdeaTag, IdeaTagResponse>()
            .Map(dest => dest.CategoryName, src => src.Category.Name);
    }
}