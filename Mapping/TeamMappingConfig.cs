using EduBridge.Contracts.Team;
using EduBridge.Entities;
using Mapster;

namespace EduBridge.Mapping;

public class TeamMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Team, TeamResponse>()
            .Map(dest => dest.LeaderName, src => $"{src.Leader.FirstName} {src.Leader.LastName}")
            .Map(dest => dest.CurrentMembers, src => src.Members.Count)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
    }
}